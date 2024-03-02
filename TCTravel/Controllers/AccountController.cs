using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TCTravel.Models;
using TCTravel.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using TCTravel.Helpers;

namespace TCTravel.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;

    // User dependency injection to pass the logger to AccountController
    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        EmailService emailService, IConfiguration configuration, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    // Authorize ensures only requests with Jwt tokens can access the associated action
    [Authorize]
    // Register user to website 
    [HttpPost("register")]
    
    //Taking in AuthModel which means, it will only need an email and password to register the user
    //These are the only two fields needed in the JSON body for the POST action
    public async Task<IActionResult> Register(AuthModel model)
    {
        // Handle invalid user input during registration process
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state during registration");
            // If the incoming data is not valid, prevent further processing
            return BadRequest("Invalid model state. Please check the provided data.");
        }

        // Use helper method to check password validity
        if (!ValidationHelper.IsPasswordValid(model.Password))
        {
            _logger.LogError("Invalid password used during registration");
            return BadRequest("Invalid password. Please ensure your password contains at least 8 characters " +
                              "and contains at least one lower-case, upper-case, digit and symbol.");
        }
        
        // User registration can proceed
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Generate an email verification token, user can now communicate with endpoint services
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Create the verification link
            //TODO Check if email is a valid email
            var verificationLink = Url.Action("VerifyEmail", "Account", new { userId = user.Id, token = token },
                Request.Scheme);

            // Send the verification email
            var emailSubject = "Email Verification for Tay Country Travel";
            var emailBody = $"Welcome to Tay Country Travel! We are pleased you are interest in our luxury travel service. Thank you for creating an account. " +
                            $"To finish the sign-up process, please verify your email by clicking the following link: \n {verificationLink}" +
                            $"\n\nKind regards,\n" +
                            $"Tay Country Travel Team";
            _emailService.SendEmail(user.Email, emailSubject, emailBody);

            //TODO check if the username is print correctly
            _logger.LogInformation($"User {user.UserName} registered successfully. Email verification link sent.");
            return Ok("Tay Country Travel's user has been sent an email verification link to finalise the user authentication process.");
        }

        _logger.LogError($"Registration failed for user {user.UserName}. Errors: {string.Join(", ", result.Errors)}");
        return BadRequest(result.Errors);
    }


    // Add an action to handle email verification
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(string userId, string token)
    {
        // Ensure userID and token parameters are not null
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            _logger.LogError("Invalid userId or token used during email verification process.");
            return BadRequest("Invalid verification request. Please check provided data.");
        }
        
        // Check if user exists before verifying the email, attempts to find user by userID asynchronously 
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _logger.LogError("User not found for email verification process.");
            return NotFound("User not found.");
        }
        
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            _logger.LogInformation("Email verification for user was successful.");
            return Ok("Email verification for Tay Country Travel was successful. Welcome new user! :-)");
        }

        _logger.LogError($"Email verification failed for user {user.UserName}. Errors: {string.Join(", ", result.Errors)}");
        return BadRequest("Email verification failed. Please try again. :-(");
    }

    // Check whether account exists or not 
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthModel model)
    {
        // Handle invalid user input during login process
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state during login.");
            // If data is not valid, prevent further processing
            return BadRequest("Invalid model state. Please check the provided data.");
        }
        
        // Asynchronous method that attempts to sign in user with email and password
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(user);
            // Generating token
            var token = GenerateJwtToken(user, roles);
            _logger.LogInformation($"Login for {model.Email} was successful.");
            // Return JWT Token for user authentication
            return Ok(new { Token = token });
        }

        _logger.LogError($"Unauthorized login attempt for {model.Email}");
        return Unauthorized("Invalid login attempt. Please try again.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out successfully.");
        return Ok("Logged out");
    }

    private string GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
        // Ensure the values used for generating the Jwt tokens are not null
        if (string.IsNullOrEmpty(_configuration["Jwt:Key"]) || string.IsNullOrEmpty(_configuration["Jwt:ExpireHours"]))
        {
            _logger.LogError("Invalid operation exception, Jwt configuration is invalid.");
            throw new InvalidOperationException("Jwt configuration is invalid.");
        }
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Add roles as claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"]));

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: expires,
            signingCredentials: creds
        );
        
        _logger.LogInformation("Jwt security token created successfully.");
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}