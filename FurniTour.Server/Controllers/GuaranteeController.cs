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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<GuaranteeModel>>> GetGuarantees()
        {
            var guarantees = await guaranteeService.GetGuarantees();
            return Ok(guarantees);
        }

        [HttpGet("{id}")]
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
        [HttpPost]
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
        [HttpPut("{id}")]
        public ActionResult UpdateGuarantee(int id, [FromBody] string status)
        {
            var guarantee = guaranteeService.GetGuarantee(id);
            if (guarantee == null)
            {
                return NotFound();
            }
            guaranteeService.UpdateGuarantee(id, status);
            return Ok();
        }

        [HttpGet("validate/{id}")]
        public ActionResult<bool> IsGuaranteeValid(int id)
        {
            var isValid = guaranteeService.IsGuaranteeValid(id);
            return Ok(isValid);
        }
    }
}
