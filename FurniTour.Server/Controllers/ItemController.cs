using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Item;
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
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var items = itemFurnitureService.getAll();
            return Ok(items);
        }

        [HttpPost("create")]
        public IActionResult AddItem([FromBody]ItemModel itemModel)
        {
            if (itemFurnitureService.AddItem(itemModel))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteItem(int id)
        {
            if (itemFurnitureService.DeleteItem(id))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            var item = itemFurnitureService.Details(id);
            if (item != null)
            {
                return Ok(item);
            }
            return BadRequest();
        }
    }
}
