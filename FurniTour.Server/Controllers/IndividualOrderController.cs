using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.IndividualOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndividualOrderController : ControllerBase
    {
        private readonly IIndividualOrderService individualOrderService;

        public IndividualOrderController(IIndividualOrderService individualOrderService)
        {
            this.individualOrderService = individualOrderService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyIndividualOrders()
        {
            var orders = await individualOrderService.GetMyIndividualOrdersAsync();
            if (orders == null)
            {
                return Unauthorized("Користувач не авторизований");
            }
            return Ok(orders);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateIndividualOrder([FromForm] IndividualOrderModel model)
        {
            var result = await individualOrderService.CreateIndividualOrderAsync(model);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Administrator,Master")]
        public async Task<IActionResult> GetAllIndividualOrders()
        {
            var orders = await individualOrderService.GetAllIndividualOrdersAsync();
            if (orders == null)
            {
                return Unauthorized("Недостатньо прав");
            }
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetIndividualOrderDetails(int id)
        {
            var order = await individualOrderService.GetIndividualOrderDetailsAsync(id);
            if (order == null)
            {
                return NotFound("Замовлення не знайдено або недостатньо прав");
            }
            return Ok(order);
        }

        [HttpPost("status/{id}/{statusId}")]
        [Authorize]
        public async Task<IActionResult> ChangeIndividualOrderStatus(int id, int statusId)
        {
            var result = await individualOrderService.ChangeIndividualOrderStatusAsync(id, statusId);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }

        [HttpPost("assign/{orderId}/{masterId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AssignMasterToOrder(int orderId, string masterId)
        {
            var result = await individualOrderService.AssignMasterToOrderAsync(orderId, masterId);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }

        [HttpPost("price/estimated/{orderId}")]
        [Authorize(Roles = "Administrator,Master")]
        public async Task<IActionResult> SetEstimatedPrice(int orderId, [FromBody] decimal price)
        {
            var result = await individualOrderService.SetEstimatedPriceAsync(orderId, price);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }

        [HttpPost("price/final/{orderId}")]
        [Authorize(Roles = "Administrator,Master")]
        public async Task<IActionResult> SetFinalPrice(int orderId, [FromBody] decimal price)
        {
            var result = await individualOrderService.SetFinalPriceAsync(orderId, price);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }

        [HttpPost("notes/{orderId}")]
        [Authorize(Roles = "Administrator,Master")]
        public async Task<IActionResult> AddMasterNotes(int orderId, [FromBody] string notes)
        {
            var result = await individualOrderService.AddMasterNotesAsync(orderId, notes);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok();
        }          [HttpGet("image/{id}")]
        public async Task<IActionResult> GetIndividualOrderImage(int id)
        {
            var context = HttpContext.RequestServices.GetService<FurniTour.Server.Data.ApplicationDbContext>();
            
            var order = await context.IndividualOrders.FindAsync(id);
            if (order?.Photo == null)
            {
                return NotFound();
            }
            
            // Визначаємо тип файлу за першими байтами
            string contentType = DetermineContentType(order.Photo);
            
            return File(order.Photo, contentType);
        }
        
        // Метод для визначення типу контенту на основі "магічних чисел"
        private string DetermineContentType(byte[] data)
        {
            if (data.Length >= 2)
            {
                // Перевіряємо сигнатуру файлу
                if (data[0] == 0xFF && data[1] == 0xD8) // JPEG
                    return "image/jpeg";
                
                if (data[0] == 0x89 && data.Length >= 4 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47) // PNG
                    return "image/png";
                
                if (data[0] == 0x47 && data.Length >= 3 && data[1] == 0x49 && data[2] == 0x46) // GIF
                    return "image/gif";
                
                if (data[0] == 0x25 && data.Length >= 4 && data[1] == 0x50 && data[2] == 0x44 && data[3] == 0x46) // PDF
                    return "application/pdf";
            }
            
            // Якщо тип не розпізнано, використовуємо загальний тип
            return "application/octet-stream";
        }
    }
}
