using FurniTour.Server.Interfaces;
using FurniTour.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemFurnitureService itemFurnitureService;
        public ItemController(IItemFurnitureService itemFurnitureService) {
            this.itemFurnitureService = itemFurnitureService;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var items = itemFurnitureService.getAll();
            return Ok(items);
        }

        [HttpPost]
        public IActionResult AddItem(ItemModel itemModel)
        {
            if (itemFurnitureService.AddItem(itemModel))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
