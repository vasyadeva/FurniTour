using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Guarantee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuaranteeController : ControllerBase
    {
        private readonly IGuaranteeService guaranteeService;

        public GuaranteeController(IGuaranteeService guaranteeService)
        {
            this.guaranteeService = guaranteeService;
        }

        [HttpGet("getall")]
        [Authorize]
        public async Task<ActionResult<List<GuaranteeModel>>> GetGuarantees()
        {
            var guarantees = await guaranteeService.GetGuarantees();
            return Ok(guarantees);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<GuaranteeModel>> GetGuarantee(int id)
        {
            var guarantee = await guaranteeService.GetGuarantee(id);
            if (guarantee == null)
            {
                return NotFound();
            }
            return Ok(guarantee);
        }

        [Authorize]
        [HttpGet("my")]
        public ActionResult<List<GuaranteeModel>> GetMyGuarantees()
        {
            var guarantees = guaranteeService.GetMyGuarantees();
            if (guarantees == null)
            {
                return Unauthorized();
            }
            return Ok(guarantees);
        }

        [Authorize]
        [HttpPost("add")]
        public ActionResult AddGuarantee([FromBody] GuaranteeAddModel model)
        {
            var result = guaranteeService.AddGuarantee(model);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }

        [Authorize]
        [HttpPost("update/{id}")]
        public ActionResult UpdateGuarantee(int id, [FromBody] string status)
        {
            guaranteeService.UpdateGuarantee(id, status);
            return Ok();
        }
    }
}
