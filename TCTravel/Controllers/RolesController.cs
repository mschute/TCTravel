using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TCTravel.Models;


namespace TCTravel.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: api/Location
        // Retrieve all Roles
        [HttpGet]
        public IActionResult GetRoles()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
                
                _logger.LogInformation("Roles retrieved successfully!");
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetRoles)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again");
            }
        }

        // Get: api/Roles/5
        // Retrieve specific roles
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                {
                    _logger.LogError($"Error, Role {roleId} not found.");
                    return NotFound("Role not found.");
                }
                
                _logger.LogInformation($"Location {roleId} retrieved successfully.");
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetRole)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // POST: api/Roles
        // Create role
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            try
            {
                var role = new IdentityRole(roleName);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role {roleName} created successfully!");
                    return Ok("Role created successfully.");
                }
                
                _logger.LogError("Error. Invalid request.");
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CreateRole)} failed with error: {ex}");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        // PUT: api/Roles/5
        // Update specific role
        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                _logger.LogError("Error. Role not found.");
                return NotFound("Role not found.");
            }

            role.Name = model.NewRoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                _logger.LogError($"Role {model.RoleId} was updated successfully");
                return Ok("Role updated successfully.");
            }

            _logger.LogError("Error. Invalid request.");
            return BadRequest(result.Errors);
        }

        // DELETE: api/Roles/5
        // Delete specific role
        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                _logger.LogError($"Error. Role {roleId} not found.");
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Role {roleId} deleted successfully!");
                return Ok("Role deleted successfully.");
            }

            _logger.LogError("Error. Invalid request.");
            return BadRequest(result.Errors);
        }

        // POST: api/Roles/assign-role-to-user
        // Assign role to user
        [HttpPost("assign-role-to-user")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                _logger.LogError($"Error. User not found.");
                return NotFound("User not found.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);

            if (!roleExists)
            {
                _logger.LogError($"Error. Role not found.");
                return NotFound("Role not found.");
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Role {model.RoleName} assigned to use {model.UserId} assigned successfully!");
                return Ok("Role assigned to user successfully.");
            }

            _logger.LogError($"Invalid request. Errors: {string.Join(", ", result.Errors)}");
            return BadRequest(result.Errors);
        }
    }