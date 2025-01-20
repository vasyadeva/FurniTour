using FurniTour.Server.Data.Entities;
using FurniTour.Server.Data;
using FurniTour.Server.Interfaces;
using Microsoft.EntityFrameworkCore;
using FurniTour.Server.Models.Item;
using FurniTour.Server.Interfaces;
using System.Linq;

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

        var allFurniture = await _context.Furnitures
            .Include(f => f.Category)
            .Include(f => f.Color)
            .Include(f => f.Master)
            .Include(f => f.Manufacturer)
            .ToListAsync();

        var recommendations = CalculateBPRRecommendations(userClicks, allFurniture);

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
        if (!userClicks.Any())
            return allFurniture.Take(10).ToList();

        // Initialize user preferences
        var userId = userClicks.First().UserId;
        var userFurnitures = userClicks.Select(c => c.Furniture).Where(f => f != null).ToList();

        var userPreferences = ExtractUserPreferences(userFurnitures);
        var (userAveragePrice, favoriteColorId, favoriteMasterId, favoriteManufacturerId) = userPreferences;

        // BPR parameters
        const int latentFactors = 50;
        const double learningRate = 0.05;
        const double regularization = 0.01;
        const int epochs = 200;

        // Initialize latent factors
        var userFactors = InitializeUserFactors(userClicks, latentFactors);
        var itemFactors = InitializeItemFactors(allFurniture, latentFactors);

        // Training loop
        for (int epoch = 0; epoch < epochs; epoch++)
        {
            foreach (var click in userClicks)
            {
                var negativeItemId = GetNegativeItemId(userClicks, allFurniture, userId, click.FurnitureId);
                if (!negativeItemId.HasValue) continue;

                UpdateFactors(
                    click,
                    negativeItemId.Value,
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

        return RankItems(
            allFurniture,
            userId,
            userFactors,
            itemFactors,
            userPreferences
        );
    }

    private (double averagePrice, int? colorId, string masterId, int? manufacturerId) ExtractUserPreferences(List<Furniture> userFurnitures)
    {
        var averagePrice = userFurnitures.Count != 0 ? (double)userFurnitures.Average(f => f.Price) : 5000.0;

        var colorId = userFurnitures
            .Where(f => f.Color != null)
            .GroupBy(f => f.Color.Id)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        var masterId = userFurnitures
            .Where(f => f.Master != null)
            .GroupBy(f => f.Master.Id)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        var manufacturerId = userFurnitures
            .Where(f => f.Manufacturer != null)
            .GroupBy(f => f.Manufacturer.Id)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        return (averagePrice, colorId, masterId, manufacturerId);
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
        var (averagePrice, favoriteColorId, favoriteMasterId, favoriteManufacturerId) = userPreferences;

        var positiveFurniture = allFurniture.First(x => x.Id == click.FurnitureId);
        var negativeFurniture = allFurniture.First(x => x.Id == negativeItemId);

        var positiveMetaFactor = CalculateMetaFactor(positiveFurniture, userPreferences);
        var negativeMetaFactor = CalculateMetaFactor(negativeFurniture, userPreferences);

        var positiveScore = DotProduct(userFactors[click.UserId], itemFactors[click.FurnitureId])
            * click.InteractionCount * positiveMetaFactor;
        var negativeScore = DotProduct(userFactors[click.UserId], itemFactors[negativeItemId])
            * negativeMetaFactor;

        var gradientMultiplier = Sigmoid(positiveScore - negativeScore);

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
        var userGradient = gradientMultiplier * (positiveFactor[index] - negativeFactor[index]) - regularization * userFactor[index];
        var positiveGradient = gradientMultiplier * userFactor[index] - regularization * positiveFactor[index];
        var negativeGradient = -gradientMultiplier * userFactor[index] - regularization * negativeFactor[index];

        userFactor[index] += learningRate * userGradient;
        positiveFactor[index] += learningRate * positiveGradient;
        negativeFactor[index] += learningRate * negativeGradient;
    }

    private List<Furniture> RankItems(
        List<Furniture> allFurniture,
        string userId,
        Dictionary<string, double[]> userFactors,
        Dictionary<int, double[]> itemFactors,
        (double averagePrice, int? colorId, string masterId, int? manufacturerId) userPreferences)
    {
        return allFurniture
            .Select(f =>
            {
                var rawScore = DotProduct(userFactors[userId], itemFactors[f.Id]);
                var metaFactor = CalculateMetaFactor(f, userPreferences);
                return new { Item = f, Score = rawScore * metaFactor };
            })
            .OrderByDescending(x => x.Score)
            .Take(10)
            .Select(x => x.Item)
            .ToList();
    }

    private double CalculateMetaFactor(
        Furniture furniture,
        (double averagePrice, int? colorId, string masterId, int? manufacturerId) userPreferences,
        int clickCount)
    {
        if (furniture == null) return 1.0;

        var (averagePrice, favoriteColorId, favoriteMasterId, favoriteManufacturerId) = userPreferences;

        var priceFactor = CalculatePriceFactor((double)furniture.Price, averagePrice);
        var colorFactor = CalculateColorFactor(furniture.Color?.Id, favoriteColorId);
        var masterFactor = CalculateMasterFactor(furniture.Master?.Id, favoriteMasterId);
        var manufacturerFactor = CalculateManufacturerFactor(furniture.Manufacturer?.Id, favoriteManufacturerId);
        var clickFactor = CalculateClickFactor(clickCount);

        return priceFactor * colorFactor * masterFactor * manufacturerFactor * clickFactor;
    }

    private double CalculateClickFactor(int clickCount)
    {
        return 1.0 + (clickCount * 0.01);
    }

    private double CalculatePriceFactor(double price, double averagePrice)
    {
        if (averagePrice <= 0) return 1.0;
        var diff = Math.Abs(price - averagePrice);
        var ratio = diff / averagePrice;
        var factor = 1.2 - ratio;
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
}