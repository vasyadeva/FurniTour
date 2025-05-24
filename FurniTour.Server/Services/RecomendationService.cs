using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Item;

namespace FurniTour.Server.Services
{
    public class RecomendationService : IRecomendationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public RecomendationService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<List<ItemViewModel>> GetRecommendationsAsync()
        {
            var user = _authService.GetUser();
            if (user == null)
                throw new ArgumentException("Користувача не знайдено");

            var userId = user.Id;

            // Перевіряємо стан рекомендацій користувача
            var userRecommendationState = await _context.UserRecomendationStates
                .FirstOrDefaultAsync(u => u.UserId == userId);

            // Якщо стан не існує, створюємо новий
            if (userRecommendationState == null)
            {
                userRecommendationState = new UserRecomendationState
                {
                    UserId = userId,
                    LastRecommendationCheck = DateTime.UtcNow
                };
                _context.UserRecomendationStates.Add(userRecommendationState);
                await _context.SaveChangesAsync();
            }

            // Перевіряємо, чи минула хвилина з останнього оновлення
            var timeSinceLastCheck = DateTime.UtcNow - userRecommendationState.LastRecommendationCheck;
            var shouldUpdateRecommendations = timeSinceLastCheck.TotalMinutes >= 1;

            // Якщо не потрібно оновлювати, спробуємо отримати з кешу
            if (!shouldUpdateRecommendations)
            {
                var cachedRecommendation = await _context.CachedRecommendations
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cachedRecommendation != null && cachedRecommendation.RecommendedFurnitureIds.Any())
                {
                    // Отримуємо товари з кешу
                    var cachedFurniture = await _context.Furnitures
                        .Where(f => cachedRecommendation.RecommendedFurnitureIds.Contains(f.Id))
                        .Include(f => f.Category)
                        .Include(f => f.Color)
                        .Include(f => f.Master)
                        .Include(f => f.Manufacturer)
                        .ToListAsync();

                    return cachedFurniture.Select(item => new ItemViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        Image = Convert.ToBase64String(item.Image),
                        Category = item.Category?.Name,
                        Color = item.Color?.Name,
                        Manufacturer = item.Manufacturer?.Name,
                        Master = item.Master?.UserName
                    }).ToList();
                }
            }

            // Оновлюємо стан перевірки рекомендацій
            userRecommendationState.LastRecommendationCheck = DateTime.UtcNow;
            _context.UserRecomendationStates.Update(userRecommendationState);

            // Завантаження кліків користувача разом із пов'язаними сутностями
            var userClicks = await _context.Clicks
                .Where(c => c.UserId == userId)
                .Include(c => c.Furniture)
                    .ThenInclude(f => f.Master)
                .Include(c => c.Furniture)
                    .ThenInclude(f => f.Manufacturer)
                .Include(c => c.Furniture)
                    .ThenInclude(f => f.Color)
                .Include(c => c.Furniture)
                    .ThenInclude(f => f.Category)
                .ToListAsync();

            // Завантаження всіх товарів
            var allFurniture = await _context.Furnitures
                .Include(f => f.Category)
                .Include(f => f.Color)
                .Include(f => f.Master)
                .Include(f => f.Manufacturer)
                .ToListAsync();

            // Обчислюємо рекомендації за допомогою BPR та матричної факторизації
            var recommendations = CalculateBPRRecommendations(userClicks, allFurniture);

            // Кешуємо рекомендації
            await CacheRecommendationsAsync(userId, recommendations);

            // Формуємо модель для відображення
            return recommendations.Select(item => new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Image = Convert.ToBase64String(item.Image),
                Category = item.Category?.Name,
                Color = item.Color?.Name,
                Manufacturer = item.Manufacturer?.Name,
                Master = item.Master?.UserName
            }).Take(10).ToList();
        }

        private async Task CacheRecommendationsAsync(string userId, List<Furniture> recommendations)
        {
            // Видаляємо старий кеш
            var existingCache = await _context.CachedRecommendations
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (existingCache != null)
            {
                _context.CachedRecommendations.Remove(existingCache);
            }

            // Створюємо новий кеш
            var newCache = new CachedRecommendation
            {
                UserId = userId,
                CachedTime = DateTime.UtcNow,
                RecommendedFurnitureIds = recommendations.Select(f => f.Id).ToList()
            };

            _context.CachedRecommendations.Add(newCache);
            await _context.SaveChangesAsync();
        }

        private List<Furniture> CalculateBPRRecommendations(List<Clicks> userClicks, List<Furniture> allFurniture)
        {
            // Якщо користувач ще не взаємодіяв – повертаємо деяку кількість товарів
            if (!userClicks.Any())
                return allFurniture.Take(10).ToList();

            // Визначення мета-параметрів користувача на основі переглянутих товарів
            var userFurnitures = userClicks.Select(c => c.Furniture).Where(f => f != null).ToList();
            var userPreferences = ExtractUserPreferences(userFurnitures);

            // Гіперпараметри моделі
            const int latentFactors = 50;
            const double learningRate = 0.05;
            const double regularization = 0.01;
            const int epochs = 200;

            // Ініціалізація латентних факторів для користувача та товарів
            var userFactors = InitializeUserFactors(userClicks, latentFactors);
            var itemFactors = InitializeItemFactors(allFurniture, latentFactors);

            // Побудова кешу даних кліків для кожного товару
            var clickDataByItem = userClicks
                .GroupBy(c => c.FurnitureId)
                .ToDictionary(
                    g => g.Key,
                    g => new ClickData
                    {
                        Count = g.Sum(c => c.InteractionCount),
                        LastTime = g.Max(c => c.LastInteractionTime)
                    }
                );



            // Навчання моделі (BPR)
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                foreach (var click in userClicks)
                {
                    int positiveId = click.FurnitureId;
                    var positiveItem = allFurniture.FirstOrDefault(f => f.Id == positiveId);
                    if (positiveItem == null)
                        continue;

                    int? negativeId = GetNegativeItemId(userClicks, allFurniture, userClicks.First().UserId, positiveId);
                    if (!negativeId.HasValue)
                        continue;

                    var negativeItem = allFurniture.FirstOrDefault(f => f.Id == negativeId.Value);
                    if (negativeItem == null)
                        continue;

                    UpdateFactors(
                        click,
                        negativeId.Value,
                        userFactors,
                        itemFactors,
                        allFurniture,
                        userPreferences,
                        learningRate,
                        regularization,
                        latentFactors
                    );
                }
            }

            // Ранжування товарів: враховуємо як базовий скор (скалярний добуток), так і метафактор,
            // що включає ефект InteractionCount
            var rankedItems = RankItems(allFurniture, userClicks.First().UserId, userFactors, itemFactors, userPreferences, clickDataByItem);


            return rankedItems;
        }

        private (double averagePrice, int? colorId, string masterId, int? manufacturerId) ExtractUserPreferences(List<Furniture> userFurnitures)
        {
            double averagePrice = userFurnitures.Any() ? userFurnitures.Average(f => (double)f.Price) : 5000.0;

            int? favoriteColorId = userFurnitures
                .Where(f => f.Color != null)
                .GroupBy(f => f.Color.Id)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            string favoriteMasterId = userFurnitures
                .Where(f => f.Master != null)
                .GroupBy(f => f.Master.Id)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            int? favoriteManufacturerId = userFurnitures
                .Where(f => f.Manufacturer != null)
                .GroupBy(f => f.Manufacturer.Id)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            return (averagePrice, favoriteColorId, favoriteMasterId, favoriteManufacturerId);
        }

        private Dictionary<string, double[]> InitializeUserFactors(List<Clicks> userClicks, int latentFactors)
        {
            return userClicks
                .Select(c => c.UserId)
                .Distinct()
                .ToDictionary(uid => uid, uid => InitializeLatentFactors(latentFactors));
        }

        private Dictionary<int, double[]> InitializeItemFactors(List<Furniture> allFurniture, int latentFactors)
        {
            return allFurniture.ToDictionary(f => f.Id, f => InitializeLatentFactors(latentFactors));
        }

        private void UpdateFactors(
     Clicks click,
     int negativeItemId,
     Dictionary<string, double[]> userFactors,
     Dictionary<int, double[]> itemFactors,
     List<Furniture> allFurniture,
     (double averagePrice, int? colorId, string masterId, int? manufacturerId) userPreferences,
     double learningRate,
     double regularization,
     int latentFactors)
        {
            var positiveFurniture = allFurniture.First(x => x.Id == click.FurnitureId);
            var negativeFurniture = allFurniture.First(x => x.Id == negativeItemId);

            // Для позитивного прикладу враховуємо кількість кліків і час останньої взаємодії.
            var positiveMetaFactor = CalculateMetaFactor(positiveFurniture, userPreferences, click.InteractionCount, click.LastInteractionTime);
            // Для негативного прикладу припускаємо, що кліків немає, тому використовуємо DateTime.MinValue.
            var negativeMetaFactor = CalculateMetaFactor(negativeFurniture, userPreferences, 0, DateTime.MinValue);

            double positiveScore = DotProduct(userFactors[click.UserId], itemFactors[click.FurnitureId])
                * click.InteractionCount * positiveMetaFactor;
            double negativeScore = DotProduct(userFactors[click.UserId], itemFactors[negativeItemId])
                * negativeMetaFactor;

            double gradientMultiplier = Sigmoid(positiveScore - negativeScore);

            for (int k = 0; k < latentFactors; k++)
            {
                UpdateFactorValues(
                    userFactors[click.UserId],
                    itemFactors[click.FurnitureId],
                    itemFactors[negativeItemId],
                    k,
                    gradientMultiplier,
                    learningRate,
                    regularization
                );
            }
        }


        private void UpdateFactorValues(
            double[] userFactor,
            double[] positiveFactor,
            double[] negativeFactor,
            int index,
            double gradientMultiplier,
            double learningRate,
            double regularization)
        {
            double userGradient = gradientMultiplier * (positiveFactor[index] - negativeFactor[index]) - regularization * userFactor[index];
            double positiveGradient = gradientMultiplier * userFactor[index] - regularization * positiveFactor[index];
            double negativeGradient = -gradientMultiplier * userFactor[index] - regularization * negativeFactor[index];

            userFactor[index] += learningRate * userGradient;
            positiveFactor[index] += learningRate * positiveGradient;
            negativeFactor[index] += learningRate * negativeGradient;
        }

        private List<Furniture> RankItems(
    List<Furniture> allFurniture,
    string userId,
    Dictionary<string, double[]> userFactors,
    Dictionary<int, double[]> itemFactors,
    (double averagePrice, int? colorId, string masterId, int? manufacturerId) userPreferences,
    Dictionary<int, ClickData> clickDataByItem) // використовується ClickData
        {
            return allFurniture
                .Select(f =>
                {
                    double baseScore = DotProduct(userFactors[userId], itemFactors[f.Id]);
                    // Якщо для товару є дані про кліки, використовуємо їх; інакше – встановлюємо базові значення.
                    int interactionCount = clickDataByItem.ContainsKey(f.Id) ? clickDataByItem[f.Id].Count : 1;
                    DateTime lastInteractionTime = clickDataByItem.ContainsKey(f.Id) ? clickDataByItem[f.Id].LastTime : DateTime.MinValue;
                    double metaFactor = CalculateMetaFactor(f, userPreferences, interactionCount, lastInteractionTime);
                    return new { Item = f, Score = baseScore * metaFactor };
                })
                .OrderByDescending(x => x.Score)
                .Take(10)
                .Select(x => x.Item)
                .ToList();
        }


        private int? GetNegativeItemId(List<Clicks> userClicks, List<Furniture> allFurniture, string userId, int positiveItemId)
        {
            var viewedItemIds = userClicks
                .Where(c => c.UserId == userId)
                .Select(c => c.FurnitureId)
                .ToList();

            var notViewedItemId = allFurniture
                .Where(f => !viewedItemIds.Contains(f.Id))
                .OrderBy(_ => Guid.NewGuid())
                .FirstOrDefault()?.Id;

            return notViewedItemId ?? viewedItemIds.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        }

        private double[] InitializeLatentFactors(int count)
        {
            var random = new Random();
            return Enumerable.Range(0, count)
                .Select(_ => random.NextDouble() * 0.1)
                .ToArray();
        }

        private double DotProduct(double[] v1, double[] v2)
        {
            return v1.Zip(v2, (a, b) => a * b).Sum();
        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        /// <summary>
        /// Обчислює загальний метафактор для товару з урахуванням його властивостей
        /// та ефекту кліків (кількість і час останньої взаємодії).
        /// </summary>
        private double CalculateMetaFactor(
            Furniture furniture,
            (double averagePrice, int? colorId, string masterId, int? manufacturerId) userPreferences,
            int clickCount,
            DateTime lastInteractionTime)
        {
            if (furniture == null) return 1.0;

            var (averagePrice, favoriteColorId, favoriteMasterId, favoriteManufacturerId) = userPreferences;

            double priceFactor = CalculatePriceFactor((double)furniture.Price, averagePrice);
            double colorFactor = CalculateColorFactor(furniture.Color?.Id, favoriteColorId);
            double masterFactor = CalculateMasterFactor(furniture.Master?.Id, favoriteMasterId);
            double manufacturerFactor = CalculateManufacturerFactor(furniture.Manufacturer?.Id, favoriteManufacturerId);

            // Якщо є кліки (тобто clickCount > 0), використовуємо обчислення з урахуванням часу,
            // інакше – повертаємо базове значення 1.0.
            double clickFactor = (clickCount > 0) ? CalculateClickFactor(clickCount, lastInteractionTime) : 1.0;

            return priceFactor * colorFactor * masterFactor * manufacturerFactor * clickFactor;
        }


        /// <summary>
        /// Обчислює фактор впливу кліків із врахуванням кількості кліків та часу останньої взаємодії.
        /// </summary>
        /// <param name="clickCount">Кількість кліків</param>
        /// <param name="lastInteractionTime">Час останнього кліку</param>
        /// <returns>Фактор, який використовується при розрахунку загального метафактора</returns>
        private double CalculateClickFactor(int clickCount, DateTime lastInteractionTime)
        {
            // Базовий фактор за кількістю кліків – наприклад, кожен клік додає 5%
            double baseClickFactor = 1.0 + (clickCount * 0.05);

            // Отримуємо часовий фактор (чим більш свіжий клік, тим він більше впливає)
            double timeFactor = CalculateTimeFactor(lastInteractionTime);

            return baseClickFactor * timeFactor;
        }


        /// <summary>
        /// Обчислює часовий фактор залежно від того, скільки часу пройшло від останньої взаємодії.
        /// Чим «свіжіший» клік (менше пройшло часу), тим вищий коефіцієнт.
        /// Наприклад, використовуємо експоненційну функцію занепаду.
        /// </summary>
        /// <param name="lastInteractionTime">Час останньої взаємодії</param>
        /// <returns>Коефіцієнт, де 1.0 – базове значення, а для свіжих кліків більше 1.0</returns>
        private double CalculateTimeFactor(DateTime lastInteractionTime)
        {
            // Якщо час не заданий (за замовчуванням), повертаємо 1.0
            if (lastInteractionTime == DateTime.MinValue)
                return 1.0;

            // Розраховуємо кількість годин, що пройшли від останньої взаємодії
            double hoursPassed = (DateTime.UtcNow - lastInteractionTime).TotalHours;

            // Параметр, який регулює швидкість занепаду (чим більший, тим швидше зменшується вага)
            double decayRate = 0.05; // можна налаштовувати

            // Експоненційний занепад: для свіжих кліків (hoursPassed ~ 0) фактор буде ~ 1.0,
            // але зі збільшенням часу значення зменшується.
            double timeFactor = Math.Exp(-decayRate * hoursPassed);

            // Щоб свіжі кліки мали додаткове підсилення, можна «підняти» цей коефіцієнт,
            // наприклад, використовуючи масштабування або додавання постійного значення.
            // Тут для прикладу домножимо на коефіцієнт (якщо потрібно – налаштуйте)
            return 1.0 + (1.0 - timeFactor);
        }


        private double CalculatePriceFactor(double price, double averagePrice)
        {
            if (averagePrice <= 0) return 1.0;
            double diff = Math.Abs(price - averagePrice);
            double ratio = diff / averagePrice;
            double factor = 1.2 - ratio;
            return Math.Clamp(factor, 0.8, 1.2);
        }

        private double CalculateColorFactor(int? itemColorId, int? favoriteColorId)
        {
            if (!itemColorId.HasValue || !favoriteColorId.HasValue) return 1.0;
            return itemColorId == favoriteColorId ? 1.1 : 1.0;
        }

        private double CalculateMasterFactor(string itemMasterId, string favoriteMasterId)
        {
            if (string.IsNullOrEmpty(itemMasterId) || string.IsNullOrEmpty(favoriteMasterId)) return 1.0;
            return itemMasterId == favoriteMasterId ? 1.1 : 1.0;
        }

        private double CalculateManufacturerFactor(int? itemManufacturerId, int? favoriteManufacturerId)
        {
            if (!itemManufacturerId.HasValue || !favoriteManufacturerId.HasValue) return 1.0;
            return itemManufacturerId == favoriteManufacturerId ? 1.1 : 1.0;
        }
    }
}

public class ClickData
{
    public int Count { get; set; }
    public DateTime LastTime { get; set; }
}
