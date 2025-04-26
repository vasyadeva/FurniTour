using FurniTour.Server.Constants;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemFurnitureService itemFurnitureService;
        private readonly IRecomendationService recomendationService;

        public ItemController(IItemFurnitureService itemFurnitureService, IRecomendationService recomendationService) {
            this.itemFurnitureService = itemFurnitureService;
            this.recomendationService = recomendationService;
        }
        [HttpGet("recommend")]
        public async Task<IActionResult> GetRecommendations()
        {
            var items = await recomendationService.GetRecommendationsAsync();
            return Ok(items);
        }


        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var items = itemFurnitureService.getAll();
            return Ok(items);
        }

        [HttpPost("getfiltereditems")]
        public IActionResult GetFilteredItems([FromBody] ItemFilterModel model)
        {
            var items = itemFurnitureService.getFilteredItems(model);
            return Ok(items);
        }

        [Authorize(Roles = (Roles.Administrator + "," + Roles.Master))]
        [HttpPost("create")]
        public async Task<IActionResult> AddItem([FromBody]ItemModel itemModel)
        {
            var state = await itemFurnitureService.AddItem(itemModel);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            else
            {
                return BadRequest(state);
            }
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var state = await itemFurnitureService.DeleteItem(id);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] ItemUpdateModel itemModel)
        {
            var state = await itemFurnitureService.Edit(itemModel.Id, itemModel);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
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

        [HttpGet("categories/getall")]
        public IActionResult GetCategories()
        {
            var categories = itemFurnitureService.GetCategories();
            return Ok(categories);
        }

        [HttpGet("colors/getall")]
        public IActionResult GetColors()
        {
            var colors = itemFurnitureService.GetColors();
            return Ok(colors);
        }

        [HttpGet("search/{description}")]
        public async Task<IActionResult> GetItemsByDescription(string description)
        {
            var items = await itemFurnitureService.GetItemsByDescriptionAsync(description);
            return Ok(items);
        }

        [HttpGet("search2/{description}")]
        public async Task<IActionResult> GetItemsByDescription2(string description)
        {
            var items = await itemFurnitureService.GetItemsByDescriptionAsync2(description);
            var serialized = System.Text.Json.JsonSerializer.Serialize(items);

            return Content(serialized, "text/plain"); // <- ключове!
        }

        [HttpGet("image/{id}")]
        public ActionResult image(int id)
        {
            byte[] bytes = itemFurnitureService.GetImage(id);
            return File(bytes, "image/jpg");
        }




    }
}
