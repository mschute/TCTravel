using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TCTravel.Helpers;
using TCTravel.Models;


namespace TCTravel.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    // Authorize only SuperAdmin to manage user roles
    [Authorize(Roles = "SuperAdmin")]
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
                
                _logger.LogInformationEx("Roles retrieved successfully");
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // Get: api/Roles/5
        // Retrieve specific roles
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(string roleId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                {
                    _logger.LogErrorEx($"Role {roleId} not found");
                    return NotFound($"Role {roleId} not found");
                }
                
                _logger.LogInformationEx($"Role {roleId} retrieved successfully");
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // POST: api/Roles
        // Create role
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogErrorEx("Invalid request");
                    return BadRequest(ModelState);
                }
                
                var role = new IdentityRole(roleName);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformationEx($"Role {roleName} created successfully");
                    return Ok($"Role {roleName} created successfully");
                }
                
                _logger.LogErrorEx($"Invalid request: {string.Join(", ", result.Errors)}");
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx($"Failed with error: {ex}");
                return StatusCode(500, $"Failed with error: {ex}");
            }
        }

        // PUT: api/Roles/5
        // Update specific role
        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogErrorEx("Invalid request");
                return BadRequest(ModelState);
            }
            
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                _logger.LogErrorEx($"Role {model.RoleId} not found");
                return NotFound($"Role {model.RoleId} not found");
            }

            role.Name = model.NewRoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformationEx($"Role {model.RoleId} was updated successfully");
                return Ok($"Role {model.RoleId} was updated successfully");
            }

            _logger.LogErrorEx($"Invalid request: {string.Join(", ", result.Errors)}");
            return BadRequest(result.Errors);
        }

        // DELETE: api/Roles/5
        // Delete specific role
        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogErrorEx("Invalid request");
                return BadRequest(ModelState);
            }
            
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                _logger.LogErrorEx($"Role {roleId} not found");
                return NotFound($"Role {roleId} not found");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformationEx($"Role {roleId} deleted successfully");
                return Ok("Role deleted successfully");
            }

            _logger.LogErrorEx($"Invalid request: {string.Join(", ", result.Errors)}");
            return BadRequest(result.Errors);
        }

        // POST: api/Roles/assign-role-to-user
        // Assign role to user
        [HttpPost("assign-role-to-user")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
        {
            
            if (!ModelState.IsValid)
            {
                _logger.LogErrorEx("Error. Invalid request.");
                return BadRequest(ModelState);
            }
            
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                _logger.LogErrorEx($"Error. User not found.");
                return NotFound("User not found.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);

            if (!roleExists)
            {
                _logger.LogErrorEx($"Error. Role not found.");
                return NotFound("Role not found.");
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (result.Succeeded)
            {
                _logger.LogInformationEx($"Role {model.RoleName} assigned to use {model.UserId} assigned successfully!");
                return Ok("Role assigned to user successfully.");
            }

            _logger.LogErrorEx($"Invalid request: {string.Join(", ", result.Errors)}");
            return BadRequest(result.Errors);
        }
    }