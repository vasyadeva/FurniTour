using FurniTour.Server.Data.Entities;
using FurniTour.Server.Data;
using FurniTour.Server.Interfaces;
using Microsoft.EntityFrameworkCore;
using FurniTour.Server.Models.Item;

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
        {
            throw new ArgumentException("User not found");
        }
        var userId = user.Id;

        var cachedRecommendation = await GetCachedRecommendationsAsync(userId);
        if (cachedRecommendation != null)
        {
            var ListModel = new List<ItemViewModel>();
            foreach (var item in cachedRecommendation)
            {
                var Manufacturer = string.Empty;
                var Master = string.Empty;
                if (item.ManufacturerId != null)
                {
                    Manufacturer = _context.Manufacturers.Where(c => c.Id == item.ManufacturerId).FirstOrDefault().Name;
                }
                if (item.MasterId != null)
                {
                    Master = _context.Users.Where(c => c.Id == item.MasterId).FirstOrDefault().UserName;
                }

                var itemModel = new ItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    Image = Convert.ToBase64String(item.Image),
                    Category = item.Category.Name,
                    Color = item.Color.Name,
                    Manufacturer = Manufacturer,
                    Master = Master

                };
                ListModel.Add(itemModel);
            }
            return ListModel;
        }
        var userClicks = await _context.Clicks
            .Where(c => c.UserId == userId)
            .Include(c => c.Furniture)
            .ToListAsync();

        var allFurniture = await _context.Furnitures
            .Include(f => f.Category)
            .Include(f => f.Color)
            .Include(f => f.Master)
            .ToListAsync();

        var recommendations = CalculateBayesianRecommendations(userClicks, allFurniture);

        await CacheRecommendationsAsync(userId, recommendations.Select(r => r.Id).ToList());

        var itemListModel = new List<ItemViewModel>();
        foreach (var item in recommendations)
        {
            var Manufacturer = string.Empty;
            var Master = string.Empty;
            if (item.ManufacturerId != null)
            {
                Manufacturer = _context.Manufacturers.Where(c => c.Id == item.ManufacturerId).FirstOrDefault().Name;
            }
            if (item.MasterId != null)
            {
                Master = _context.Users.Where(c => c.Id == item.MasterId).FirstOrDefault().UserName;
            }

            var itemModel = new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Image = Convert.ToBase64String(item.Image),
                Category = item.Category.Name,
                Color = item.Color.Name,
                Manufacturer = Manufacturer,
                Master = Master

            };
            itemListModel.Add(itemModel);
        }
        return itemListModel;
    }

    /// <summary>
    /// Calculate Bayesian recommendations based on user clicks and other factors:
    /// P(A∣B) = P(B∣A)⋅P(A)​ / P(B)
    /// </summary>
    /// <param name="userClicks"></param>
    /// <param name="allFurniture"></param>
    /// <returns>
    /// List of recommended furniture
    /// </returns>
    private List<Furniture> CalculateBayesianRecommendations(List<Clicks> userClicks, List<Furniture> allFurniture)
    {
        var totalClicks = userClicks.Sum(c => c.InteractionCount);
        var priorProbability = 1.0 / allFurniture.Count;

        var posteriorProbabilities = allFurniture.Select(furniture =>
        {
            var furnitureClicks = userClicks.Where(c => c.FurnitureId == furniture.Id).Sum(c => c.InteractionCount);
            var likelihood = CalculateLikelihood(furniture, userClicks); // P(B∣A)
            var evidence = CalculateEvidence(userClicks, allFurniture); // P(B)
            var posterior = (likelihood * priorProbability) / evidence;
            return new { Furniture = furniture, Probability = posterior };
        }).OrderByDescending(item => item.Probability);

        return posteriorProbabilities.Take(10).Select(item => item.Furniture).ToList();
    }

    /// <summary>
    /// Calculate likelihood of a furniture being recommended based on user clicks,
    /// category, color, master, master rating, manufacturer, manufacturer rating
    /// </summary>
    /// <param name="furniture"></param>
    /// <param name="userClicks"></param>
    /// <returns></returns>
    private double CalculateLikelihood(Furniture furniture, List<Clicks> userClicks)
    {
        var totalClicks = userClicks.Sum(c => c.InteractionCount) + 6.0; // Laplacian smoothing

        var categoryLikelihood = (userClicks.Count(c => c.Furniture.CategoryId == furniture.CategoryId) + 1.0) / totalClicks;
        var colorLikelihood = (userClicks.Count(c => c.Furniture.ColorId == furniture.ColorId) + 1.0) / totalClicks;
        var masterLikelihood = 1.0 / totalClicks;
        var masterRatingLikelihood = 1.0 / 5.0;

        var manufacturerLikelihood = 1.0 / totalClicks;
        var manufacturerRatingLikelihood = 1.0 / 5.0;

        if (furniture.MasterId == null)
        {
            manufacturerLikelihood = (userClicks.Count(c => c.Furniture.ManufacturerId == furniture.ManufacturerId) + 1.0) / totalClicks;
            var manufacturerReviews = _context.ManufacturerReviews.Where(c => c.ManufacturerId == furniture.ManufacturerId).ToList();
            var manufacturerRating = manufacturerReviews.Count > 0 ? manufacturerReviews.Average(r => r.Rating) : 1;
            manufacturerRatingLikelihood = (manufacturerRating - 1) / 4.0 + 0.1; // avoid zero
        }
        else
        {
            masterLikelihood = (userClicks.Count(c => c.Furniture.MasterId == furniture.MasterId) + 1.0) / totalClicks;
            var masterReviews = _context.MasterReviews.Where(c => c.MasterId == furniture.MasterId).ToList();
            var masterRating = masterReviews.Count > 0 ? masterReviews.Average(r => r.Rating) : 1;
            masterRatingLikelihood = (masterRating - 1) / 4.0 + 0.1; // avoid zero
        }
        

        var normalizedClickWeight = (userClicks.Where(c => c.FurnitureId == furniture.Id).Sum(c => c.InteractionCount)+1) / (double)totalClicks;
        // Geometric mean of all likelihoods
        return Math.Pow(categoryLikelihood * colorLikelihood * masterLikelihood * masterRatingLikelihood * manufacturerLikelihood * manufacturerRatingLikelihood, 1.0 / 6.0) * normalizedClickWeight;
    }


    private double CalculateEvidence(List<Clicks> userClicks, List<Furniture> allFurniture)
    {
        return allFurniture.Sum(furniture =>
        {
            var likelihood = CalculateLikelihood(furniture, userClicks);
            var priorProbability = 1.0 / allFurniture.Count;
            return likelihood * priorProbability;
        });
    }


private async Task<List<Furniture>> GetCachedRecommendationsAsync(string userId)
    {
        var cachedRecommendation = await _context.CachedRecommendations
            .Where(cr => cr.UserId == userId && cr.CachedTime > DateTime.UtcNow.AddMinutes(5))
            .OrderByDescending(cr => cr.CachedTime)
            .FirstOrDefaultAsync();

        if (cachedRecommendation != null)
        {
            return await _context.Furnitures
                .Where(f => cachedRecommendation.RecommendedFurnitureIds.Contains(f.Id))
                .ToListAsync();
        }

        return null;
    }

    private async Task CacheRecommendationsAsync(string userId, List<int> recommendedFurnitureIds)
    {
        var cachedRecommendation = new CachedRecommendation
        {
            UserId = userId,
            CachedTime = DateTime.UtcNow,
            RecommendedFurnitureIds = recommendedFurnitureIds
        };

        _context.CachedRecommendations.Add(cachedRecommendation);
        await _context.SaveChangesAsync();
    }
}
