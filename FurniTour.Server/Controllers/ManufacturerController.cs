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
    
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerService manufacturerService;
        public ManufacturerController(IManufacturerService manufacturerService)
        {
            this.manufacturerService = manufacturerService;
        }

        //[Authorize(Roles = Roles.Administrator + "," + Roles.Master)]
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var manufacturers = manufacturerService.GetManufacturersList();
            return Ok(manufacturers);
        }

        [Authorize(Roles = Roles.Administrator)]
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

        [Authorize(Roles = Roles.Administrator)]
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

        [Authorize(Roles = Roles.Administrator)]
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

        [Authorize(Roles = Roles.Administrator)]
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
