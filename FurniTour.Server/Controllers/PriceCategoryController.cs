using FurniTour.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceCategoryController : ControllerBase
    {
        private readonly IIndividualOrderService individualOrderService;

        public PriceCategoryController(IIndividualOrderService individualOrderService)
        {
            this.individualOrderService = individualOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPriceCategories()
        {
            var categories = await individualOrderService.GetPriceCategoriesAsync();
            return Ok(categories);
        }
    }
}
