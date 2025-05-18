using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Item;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FurnitureReviewController : ControllerBase
    {
        private readonly IItemFurnitureService _itemService;

        public FurnitureReviewController(IItemFurnitureService itemService)
        {
            _itemService = itemService;
        }        [HttpGet("reviews/{itemId}")]
        public async Task<IActionResult> GetReviews(int itemId)
        {
            var reviews = await _itemService.GetFurnitureReviews(itemId);
            return Ok(reviews);
        }

        [HttpGet("summary/{itemId}")]
        public async Task<IActionResult> GetReviewSummary(int itemId)
        {
            var summary = await _itemService.GetReviewsSummary(itemId);
            return Ok(summary);
        }

        [HttpPost("reviews")]
        public async Task<IActionResult> AddReview(AddFurnitureReviewModel model)
        {
            var result = await _itemService.AddItemReview(model);
            if (string.IsNullOrEmpty(result))
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [HttpGet("additionalImage/{photoId}")]
        public IActionResult GetAdditionalImage(int photoId)
        {
            var imageData = _itemService.GetAdditionalImage(photoId);
            if (imageData == null)
            {
                return NotFound();
            }
            return File(imageData, "image/jpeg");
        }

        [HttpGet("count-photos/{itemId}")]
        public async Task<IActionResult> CountAdditionalPhotos(int itemId)
        {
            var count = await _itemService.GetAdditionalPhotoCount(itemId);
            return Ok(new { count });
        }
    }
}
