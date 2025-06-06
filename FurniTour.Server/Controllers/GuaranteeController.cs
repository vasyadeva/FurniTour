using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Guarantee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FurniTour.Server.Constants;

namespace FurniTour.Server.Controllers
{
    public class GuaranteeBulkUpdateModel
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty; 
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GuaranteeController : ControllerBase
    {
        private readonly IGuaranteeService guaranteeService;
        private readonly ILogger<GuaranteeController> logger;


        public GuaranteeController(IGuaranteeService guaranteeService, ILogger<GuaranteeController> logger)
        {
            this.guaranteeService = guaranteeService ?? throw new ArgumentNullException(nameof(guaranteeService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("getall")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult<List<GuaranteeModel>>> GetGuarantees()
        {
            var guarantees = await guaranteeService.GetGuarantees();
            return Ok(guarantees);
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public async Task<ActionResult<GuaranteeModel>> GetGuarantee(int id)
        {
            // Отримуємо ім'я користувача і його роль
            var userName = User.Identity.Name;
            var isAdmin = User.IsInRole(Roles.Administrator);
            
            var guarantee = await guaranteeService.GetGuarantee(id);
            if (guarantee == null)
            {
                return NotFound();
            }
            
            // Перевіряємо, чи це адміністратор або власник гарантії
            if (isAdmin || guarantee.UserName == userName)
            {
                return Ok(guarantee);
            }
            
            return Forbid();
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
        }        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult> AddGuarantee([FromBody] GuaranteeAddModel model)
        {
  
            logger.LogInformation($"Received guarantee request: {JsonSerializer.Serialize(model)}");
            
            if (model == null)
            {
                return BadRequest("Request cannot be null");
            }
            

            if (model.IsIndividualOrder)
            {
                if (!model.IndividualOrderId.HasValue)
                {
                    return BadRequest("Individual order ID is required");
                }
                
                logger.LogInformation($"Processing individual order guarantee: IndividualOrderId={model.IndividualOrderId}");
            }
            else
            {
                if (!model.OrderId.HasValue)
                {
                    return BadRequest("Order ID is required");
                }
                
                logger.LogInformation($"Processing regular order guarantee: OrderId={model.OrderId}, Items Count: {model.Items?.Count ?? 0}");
                
 
                if (model.Items == null || model.Items.Count == 0)
                {
                    return BadRequest("At least one item must be selected for regular orders");
                }
                
     
                var invalidItems = model.Items.Where(id => id <= 0).ToList();
                if (invalidItems.Any())
                {
                    return BadRequest($"Invalid item IDs detected: {string.Join(", ", invalidItems)}");
                }
                
                if (model.Items != null)
                {
                    logger.LogInformation($"Items values: {string.Join(", ", model.Items)}");
                }
            }
            
            var result = await guaranteeService.AddGuarantee(model);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("update/{id}")]
        public ActionResult UpdateGuarantee(int id, [FromBody] string status)
        {
            if (!GuaranteeStatusConst.ValidStatuses.Contains(status))
            {
                return BadRequest("Invalid guarantee status");
            }
            
            guaranteeService.UpdateGuarantee(id, status);
            return Ok();
        }
        
        [HttpGet("statuses")]
        [Authorize(Roles = Roles.Administrator)]
        public ActionResult<List<string>> GetStatuses()
        {
            return Ok(GuaranteeStatusConst.ValidStatuses);
        }
        
        [HttpGet("admin/details/{id}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult<GuaranteeModel>> GetGuaranteeAdminDetails(int id)
        {
            var guarantee = await guaranteeService.GetGuarantee(id);
            if (guarantee == null)
            {
                return NotFound();
            }
            return Ok(guarantee);
        }

        [HttpGet("admin/statistics")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult> GetGuaranteeStatistics()
        {
            try
            {
                var guarantees = await guaranteeService.GetGuarantees();
                
                // Compute statistics
                var statistics = new
                {
                    Total = guarantees.Count,
                    ByStatus = GuaranteeStatusConst.ValidStatuses.ToDictionary(
                        status => status,
                        status => guarantees.Count(g => g.Status == status)
                    ),
                    LatestGuarantees = guarantees.OrderByDescending(g => g.DateCreated).Take(5),
                    UserCount = guarantees.Select(g => g.UserName).Distinct().Count()
                };
                
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving guarantee statistics");
                return StatusCode(500, "Не вдалося отримати статистику гарантій");
            }
        }

        [HttpGet("admin/filter")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult<List<GuaranteeModel>>> GetFilteredGuarantees(
            [FromQuery] string? status = null,
            [FromQuery] string? user = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var guarantees = await guaranteeService.GetGuarantees();
                
                // Apply filters
                if (!string.IsNullOrEmpty(status))
                {
                    guarantees = guarantees.Where(g => g.Status == status).ToList();
                }
                
                if (!string.IsNullOrEmpty(user))
                {
                    guarantees = guarantees.Where(g => g.UserName == user).ToList();
                }
                
                if (dateFrom.HasValue)
                {
                    guarantees = guarantees.Where(g => g.DateCreated >= dateFrom.Value).ToList();
                }
                
                if (dateTo.HasValue)
                {
                    // Include the whole day
                    var endDate = dateTo.Value.AddDays(1).AddTicks(-1);
                    guarantees = guarantees.Where(g => g.DateCreated <= endDate).ToList();
                }
                
                return Ok(guarantees);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error filtering guarantees");
                return StatusCode(500, "Не вдалося відфільтрувати гарантії");
            }
        }

       
        [HttpGet("admin/users")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult<List<string>>> GetGuaranteeUsers()
        {
            try
            {
                var guarantees = await guaranteeService.GetGuarantees();
                var users = guarantees.Select(g => g.UserName).Distinct().ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving guarantee users");
                return StatusCode(500, "Не вдалося отримати список користувачів");
            }
        }

        [HttpPost("admin/bulk-update")]
        [Authorize(Roles = Roles.Administrator)]
        public ActionResult BulkUpdateGuarantees([FromBody] List<GuaranteeBulkUpdateModel> updates)
        {
            try
            {
                if (updates == null || !updates.Any())
                {
                    return BadRequest("Не вказані гарантії для оновлення");
                }
                

                foreach (var update in updates)
                {
                    if (!GuaranteeStatusConst.ValidStatuses.Contains(update.Status))
                    {
                        return BadRequest($"Недійсний статус гарантії: {update.Status}");
                    }
                }
                

                foreach (var update in updates)
                {
                    guaranteeService.UpdateGuarantee(update.Id, update.Status);
                }
                
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error bulk updating guarantees");
                return StatusCode(500, "Помилка при масовому оновленні гарантій");
            }
        }
    }
}
