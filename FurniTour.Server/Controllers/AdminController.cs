using FurniTour.Server.Constants;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region Users Management

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("Користувача не знайдено");

            return Ok(user);
        }

        [HttpPut("users/role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.UpdateUserRoleAsync(model.UserId, model.RoleId);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok();
        }

        [HttpGet("users/{id}/can-delete")]
        public async Task<IActionResult> CanDeleteUser(string id)
        {
            var canDelete = await _adminService.CanDeleteUserAsync(id);
            return Ok(new { canDelete });
        }        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok();
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _adminService.GetAllRolesAsync();
            return Ok(roles);
        }

        #endregion

        #region Colors Management

        [HttpGet("colors")]
        public async Task<IActionResult> GetAllColors()
        {
            var colors = await _adminService.GetAllColorsAsync();
            return Ok(colors);
        }

        [HttpGet("colors/{id}")]
        public async Task<IActionResult> GetColorById(int id)
        {
            var color = await _adminService.GetColorByIdAsync(id);
            if (color == null)
                return NotFound("Колір не знайдено");

            return Ok(color);
        }

        [HttpPost("colors")]
        public async Task<IActionResult> CreateColor([FromBody] ColorCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.CreateColorAsync(model);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok(new { message = "Колір успішно створено" });
        }

        [HttpPut("colors")]
        public async Task<IActionResult> UpdateColor([FromBody] ColorUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.UpdateColorAsync(model);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok(new { message = "Колір успішно оновлено" });
        }

        [HttpGet("colors/{id}/in-use")]
        public async Task<IActionResult> IsColorInUse(int id)
        {
            var inUse = await _adminService.IsColorInUseAsync(id);
            return Ok(new { inUse });
        }

        [HttpDelete("colors/{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var result = await _adminService.DeleteColorAsync(id);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok(new { message = "Колір успішно видалено" });
        }

        #endregion

        #region Categories Management

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _adminService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _adminService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound("Категорію не знайдено");

            return Ok(category);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.CreateCategoryAsync(model);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok();
        }

        [HttpPut("categories")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.UpdateCategoryAsync(model);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok();
        }

        [HttpGet("categories/{id}/in-use")]
        public async Task<IActionResult> IsCategoryInUse(int id)
        {
            var inUse = await _adminService.IsCategoryInUseAsync(id);
            return Ok(new { inUse });
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _adminService.DeleteCategoryAsync(id);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok();
        }        
        #endregion

        #region Manufacturers Management

        [HttpGet("manufacturers")]
        public async Task<IActionResult> GetAllManufacturers()
        {
            var manufacturers = await _adminService.GetAllManufacturersAsync();
            return Ok(manufacturers);
        }

        [HttpGet("manufacturers/{id}")]
        public async Task<IActionResult> GetManufacturerById(int id)
        {
            var manufacturer = await _adminService.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
                return NotFound("Виробника не знайдено");

            return Ok(manufacturer);
        }

        [HttpPost("manufacturers")]
        public async Task<IActionResult> CreateManufacturer([FromBody] ManufacturerCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.CreateManufacturerAsync(model);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok();
        }

        [HttpPut("manufacturers")]
        public async Task<IActionResult> UpdateManufacturer([FromBody] ManufacturerUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.UpdateManufacturerAsync(model);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok();
        }

        [HttpGet("manufacturers/{id}/in-use")]
        public async Task<IActionResult> IsManufacturerInUse(int id)
        {
            var inUse = await _adminService.IsManufacturerInUseAsync(id);
            return Ok(new { inUse });
        }

        [HttpDelete("manufacturers/{id}")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            var result = await _adminService.DeleteManufacturerAsync(id);
            if (!result.IsNullOrEmpty())
                return BadRequest(result);

            return Ok();
        }
        #endregion
    }
}