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
        private readonly Random _random;

        public RecomendationService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
            _random = new Random();
        }

        /// <summary>
        /// Основний метод, що повертає рекомендовані товари для користувача.
        /// </summary>
        public async Task<List<ItemViewModel>> GetRecommendationsAsync()
        {
            var user = _authService.GetUser();
            if (user == null)
                throw new ArgumentException("Користувача не знайдено");

            // Отримуємо історію кліків користувача із включеними пов’язаними сутностями
            var userClicks = await _context.Clicks
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Furniture)
                    .ThenInclude(f => f.Category)
                .Include(c => c.Furniture)
                    .ThenInclude(f => f.Color)
                .Include(c => c.Furniture)
                    .ThenInclude(f => f.Master)
                .Include(c => c.Furniture)
                    .ThenInclude(f => f.Manufacturer)
                .ToListAsync();

            // Отримуємо всі товари для розрахунку
            var allFurniture = await _context.Furnitures
                .Include(f => f.Category)
                .Include(f => f.Color)
                .Include(f => f.Master)
                .Include(f => f.Manufacturer)
                .ToListAsync();
            // Розраховуємо рекомендації за допомогою BPR та матричної факторизації
            var recommendations = CalculateBPRRecommendations(user.Id, userClicks, allFurniture);

            // Формуємо view model для повернення клієнту
            var result = recommendations.Select(furniture => new ItemViewModel
            {
                Id = furniture.Id,
                Name = furniture.Name,
                Description = furniture.Description,
                Price = furniture.Price,
                Image = Convert.ToBase64String(furniture.Image),
                Category = furniture.Category?.Name,
                Color = furniture.Color?.Name,
                Manufacturer = furniture.Manufacturer?.Name,
                Master = furniture.Master?.UserName
            }).ToList();

            return result;
        }

        /// <summary>
        /// Метод, що виконує навчання BPR-моделі та ранжування товарів.
        /// </summary>
        private List<Furniture> CalculateBPRRecommendations(string userId, List<Clicks> userClicks, List<Furniture> allFurniture)
        {
            // Якщо користувач ще не взаємодіяв – повертаємо випадкові товари
            if (userClicks == null || userClicks.Count == 0)
            {
                return allFurniture
                    .OrderBy(x => _random.Next())
                    .Take(10)
                    .ToList();
            }

            // Визначення «мета-параметрів» користувача на основі переглянутих товарів
            var userFurnitures = userClicks.Select(c => c.Furniture).Where(f => f != null).ToList();
            var userPreferences = ExtractUserPreferences(userFurnitures);

            // Гіперпараметри моделі
            const int latentDim = 50;
            const double learningRate = 0.05;
            const double regularization = 0.01;
            const int epochs = 200;

            // Ініціалізація латентних факторів: для користувача та для кожного товару
            var userFactors = new Dictionary<string, double[]>
            {
                [userId] = InitializeVector(latentDim)
            };

            var itemFactors = allFurniture
                .ToDictionary(f => f.Id, f => InitializeVector(latentDim));

            // Навчання за епохами: для кожного позитивного кліку вибираємо негативний приклад
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                foreach (var click in userClicks)
                {
                    int positiveId = click.FurnitureId;
                    var positiveItem = allFurniture.FirstOrDefault(f => f.Id == positiveId);
                    if (positiveItem == null)
                        continue;

                    int? negativeId = SampleNegativeItem(userId, positiveId, userClicks, allFurniture);
                    if (!negativeId.HasValue)
                        continue;

                    var negativeItem = allFurniture.FirstOrDefault(f => f.Id == negativeId.Value);
                    if (negativeItem == null)
                        continue;

                    UpdateLatentFactors(
                        userFactors[userId],
                        itemFactors[positiveId],
                        itemFactors[negativeId.Value],
                        positiveItem,
                        negativeItem,
                        userPreferences,
                        learningRate,
                        regularization,
                        latentDim);
                }
            }

            // Ранжування товарів для користувача: комбінуємо базову оцінку (скалярний добуток) з метафакторами
            var rankedItems = allFurniture
                .Select(item =>
                {
                    double baseScore = DotProduct(userFactors[userId], itemFactors[item.Id]);
                    double metaFactor = CalculateMetaFactor(item, userPreferences);
                    return new { Item = item, Score = baseScore * metaFactor };
                })
                .OrderByDescending(x => x.Score)
                .Take(10)
                .Select(x => x.Item)
                .ToList();

            return rankedItems;
        }

        /// <summary>
        /// Визначає «мета-параметри» користувача: середню ціну, найпопулярніший колір, майстра та виробника.
        /// </summary>
        private (double AveragePrice, int? FavoriteColorId, string FavoriteMasterId, int? FavoriteManufacturerId)
            ExtractUserPreferences(List<Furniture> userFurnitures)
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

        /// <summary>
        /// Ініціалізує вектор латентних факторів випадковими значеннями.
        /// </summary>
        private double[] InitializeVector(int dimension)
        {
            var vector = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                vector[i] = _random.NextDouble() * 0.1;
            }
            return vector;
        }

        /// <summary>
        /// Обчислює скалярний добуток двох векторів.
        /// </summary>
        private double DotProduct(double[] vectorA, double[] vectorB)
        {
            double sum = 0.0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                sum += vectorA[i] * vectorB[i];
            }
            return sum;
        }

        /// <summary>
        /// Вибирає негативний приклад для заданого позитивного товару. 
        /// Негативним вважається товар, з яким користувач не взаємодіяв.
        /// </summary>
        private int? SampleNegativeItem(string userId, int positiveItemId, List<Clicks> userClicks, List<Furniture> allFurniture)
        {
            // Збираємо список товарів, з якими користувач вже взаємодіяв
            var interactedIds = userClicks
                .Where(c => c.UserId == userId)
                .Select(c => c.FurnitureId)
                .ToHashSet();

            // Вибираємо випадковий товар, якого немає у взаємодіях
            var negativeCandidates = allFurniture.Where(f => !interactedIds.Contains(f.Id)).ToList();
            if (negativeCandidates.Any())
            {
                int index = _random.Next(negativeCandidates.Count);
                return negativeCandidates[index].Id;
            }

            // Якщо всі товари взаємодіяли – повертаємо випадковий товар, відмінний від позитивного
            return allFurniture
                .Where(f => f.Id != positiveItemId)
                .OrderBy(_ => _random.Next())
                .Select(f => f.Id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Оновлює латентні фактори користувача та товарів згідно з BPR-правилом.
        /// Обчислення включає метафактори, що враховують властивості товару.
        /// </summary>
        private void UpdateLatentFactors(
            double[] userVector,
            double[] positiveVector,
            double[] negativeVector,
            Furniture positiveItem,
            Furniture negativeItem,
            (double AveragePrice, int? FavoriteColorId, string FavoriteMasterId, int? FavoriteManufacturerId) userPreferences,
            double learningRate,
            double regularization,
            int dimension)
        {
            // Обчислюємо метафактори (оцінка відповідності метаданих)
            double metaPositive = CalculateMetaFactor(positiveItem, userPreferences);
            double metaNegative = CalculateMetaFactor(negativeItem, userPreferences);

            // Розраховуємо різницю «скорових» оцінок для позитивного та негативного товарів
            double posScore = DotProduct(userVector, positiveVector) * metaPositive;
            double negScore = DotProduct(userVector, negativeVector) * metaNegative;
            double x_uij = posScore - negScore;

            // Обчислюємо значення сигмоїди
            double sigmoid = 1.0 / (1.0 + Math.Exp(-x_uij));

            // Оновлення векторів латентних факторів за правилом градієнтного спуску
            for (int f = 0; f < dimension; f++)
            {
                double userGrad = sigmoid * (positiveVector[f] - negativeVector[f]) - regularization * userVector[f];
                double posGrad = sigmoid * userVector[f] - regularization * positiveVector[f];
                double negGrad = -sigmoid * userVector[f] - regularization * negativeVector[f];

                userVector[f] += learningRate * userGrad;
                positiveVector[f] += learningRate * posGrad;
                negativeVector[f] += learningRate * negGrad;
            }
        }

        /// <summary>
        /// Обчислює загальний метафактор для товару з урахуванням його властивостей:
        /// ціни, кольору, майстра та виробника.
        /// </summary>
        private double CalculateMetaFactor(Furniture furniture,
            (double AveragePrice, int? FavoriteColorId, string FavoriteMasterId, int? FavoriteManufacturerId) userPreferences)
        {
            // Фактор ціни
            double priceFactor = CalculatePriceFactor(furniture.Price, userPreferences.AveragePrice);

            // Фактор кольору: якщо колір відповідає найпопулярнішому – невелике збільшення рейтингу
            double colorFactor = (furniture.Color != null && userPreferences.FavoriteColorId.HasValue &&
                                  furniture.Color.Id == userPreferences.FavoriteColorId.Value) ? 1.1 : 1.0;

            // Фактор майстра
            double masterFactor = (furniture.Master != null && !string.IsNullOrEmpty(furniture.Master.Id) &&
                                   furniture.Master.Id == userPreferences.FavoriteMasterId) ? 1.1 : 1.0;

            // Фактор виробника
            double manufacturerFactor = (furniture.Manufacturer != null && userPreferences.FavoriteManufacturerId.HasValue &&
                                         furniture.Manufacturer.Id == userPreferences.FavoriteManufacturerId.Value) ? 1.1 : 1.0;

            return priceFactor * colorFactor * masterFactor * manufacturerFactor;
        }

        /// <summary>
        /// Обчислює фактор відповідності ціни. Чим ближче ціна товару до середньої ціни користувача,
        /// тим вищий коефіцієнт (обмежений в діапазоні [0.8, 1.2]).
        /// </summary>
        private double CalculatePriceFactor(decimal price, double averagePrice)
        {
            if (averagePrice <= 0)
                return 1.0;

            double diff = Math.Abs((double)price - averagePrice);
            double ratio = diff / averagePrice;
            double factor = 1.2 - ratio;
            return Math.Clamp(factor, 0.8, 1.2);
        }
    }
}
