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
        public ItemController(IItemFurnitureService itemFurnitureService) {
            this.itemFurnitureService = itemFurnitureService;
        }
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var items = itemFurnitureService.getAll();
            return Ok(items);
        }

        [Authorize(Roles = Roles.Administrator)]
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
        public async Task<IActionResult> Edit([FromBody] ItemViewModel itemModel)
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
    }
}
