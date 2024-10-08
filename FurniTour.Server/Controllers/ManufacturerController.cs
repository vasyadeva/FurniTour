using FurniTour.Server.Constants;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Manufacturer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerService manufacturerService;
        public ManufacturerController(IManufacturerService manufacturerService)
        {
            this.manufacturerService = manufacturerService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var manufacturers = manufacturerService.GetManufacturersList();
            return Ok(manufacturers);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var manufacturer = await manufacturerService.GetManufacturer(id);
            if (manufacturer == null)
            {
                return NotFound();
            }
            return Ok(manufacturer);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody] AddManufacturerModel model)
        {
            var state = await manufacturerService.AddManufacturer(model);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAsync( [FromBody] ManufacturerModel model)
        {
            var state = await manufacturerService.UpdateManufacturer( model);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var state = await manufacturerService.DeleteManufacturer(id);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }
    }
}
