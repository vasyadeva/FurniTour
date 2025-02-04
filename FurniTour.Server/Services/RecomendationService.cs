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
            }).ToList();
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

            // Побудова кешу загальної кількості кліків для кожного товару (сумуємо всі InteractionCount)
            var clickCountByItem = userClicks
                .GroupBy(c => c.FurnitureId)
                .ToDictionary(g => g.Key, g => g.Sum(c => c.InteractionCount));

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
            var rankedItems = RankItems(allFurniture, userClicks.First().UserId, userFactors, itemFactors, userPreferences, clickCountByItem);

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

            // Використовуємо InteractionCount для позитивного прикладу; для негативного – припустимо, що кліків немає (0)
            var positiveMetaFactor = CalculateMetaFactor(positiveFurniture, userPreferences, click.InteractionCount);
            var negativeMetaFactor = CalculateMetaFactor(negativeFurniture, userPreferences, 0);

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
            Dictionary<int, int> clickCountByItem)
        {
            return allFurniture
                .Select(f =>
                {
                    double baseScore = DotProduct(userFactors[userId], itemFactors[f.Id]);
                    // Якщо для товару є дані про кількість кліків, використовуємо їх; інакше – 1
                    int interactionCount = clickCountByItem.ContainsKey(f.Id) ? clickCountByItem[f.Id] : 1;
                    double metaFactor = CalculateMetaFactor(f, userPreferences, interactionCount);
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
        /// Обчислює загальний метафактор для товару з урахуванням його властивостей (ціна, колір, майстер, виробник)
        /// та ефекту кількості кліків (InteractionCount).
        /// </summary>
        private double CalculateMetaFactor(
            Furniture furniture,
            (double averagePrice, int? colorId, string masterId, int? manufacturerId) userPreferences,
            int clickCount)
        {
            if (furniture == null) return 1.0;

            var (averagePrice, favoriteColorId, favoriteMasterId, favoriteManufacturerId) = userPreferences;

            double priceFactor = CalculatePriceFactor((double)furniture.Price, averagePrice);
            double colorFactor = CalculateColorFactor(furniture.Color?.Id, favoriteColorId);
            double masterFactor = CalculateMasterFactor(furniture.Master?.Id, favoriteMasterId);
            double manufacturerFactor = CalculateManufacturerFactor(furniture.Manufacturer?.Id, favoriteManufacturerId);
            double clickFactor = CalculateClickFactor(clickCount);

            return priceFactor * colorFactor * masterFactor * manufacturerFactor * clickFactor;
        }

        private double CalculateClickFactor(int clickCount)
        {
            // Збільшення ваги на кожен клік (лінійно або за допомогою іншої функції)
            return 1.0 + (clickCount * 0.01);
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
