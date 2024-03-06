using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TCTravel.Models;
using TCTravel.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

    // Used dependency injection to pass the logger to AccountController
    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        EmailService emailService, IConfiguration configuration, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }
    
    // Method to register user to website using password and email
    [HttpPost("register")]
    //Passing AuthNMode to allow user registration through email and password
    public async Task<IActionResult> Register(AuthModel model)
    {
        // Handle invalid user input during registration process
        if (!ModelState.IsValid)
        {
            _logger.LogErrorEx("Invalid request");
            // If the incoming data is not valid, prevent further processing
            return BadRequest(ModelState);
        }

        // Use helper method to check password validity
        if (!ValidationHelper.IsPasswordValid(model.Password))
        {
            _logger.LogErrorEx("Invalid password used during registration");
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
            var verificationLink = Url.Action("VerifyEmail", "Account", new { userId = user.Id, token = token },
                Request.Scheme);

            // Send the verification email
            var emailSubject = "Email Verification for Tay Country Travel";
            var emailBody = $"Welcome to Tay Country Travel! We are pleased you are interest in our luxury travel service. Thank you for creating an account. " +
                            $"To finish the sign-up process, please verify your email by clicking the following link: \n {verificationLink}" +
                            $"\n\nKind regards,\n" +
                            $"Tay Country Travel Team";
            _emailService.SendEmail(user.Email, emailSubject, emailBody);
            
            _logger.LogInformationEx($"User {user.UserName} registered successfully. Email verification link sent");
            return Ok($"User {user.UserName} registered successfully. Email verification link sent");
        }

        _logger.LogErrorEx($"Registration failed for user {user.UserName}. Errors: {string.Join(", ", result.Errors)}");
        return BadRequest(result.Errors);
    }
    
    // Action to handle email verification following registration
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(string userId, string token)
    {
        // Ensure userID and token parameters are not null
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            _logger.LogErrorEx("Invalid userId or token used during email verification process");
            return BadRequest("Invalid verification request. Please check provided data");
        }
        
        // Check if user exists before verifying the email, attempts to find user by userID asynchronously 
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _logger.LogErrorEx("User not found");
            return NotFound("User not found");
        }
        
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            _logger.LogInformationEx("Email verification successful");
            return Ok("Email verification for Tay Country Travel was successful! Welcome new user!");
        }

        _logger.LogErrorEx($"Failed with error: {string.Join(", ", result.Errors)}");
        return BadRequest("Email verification failed. Please try again.");
    }

    // Log user into account
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthModel model)
    {
        // Handle invalid user input during login process
        if (!ModelState.IsValid)
        {
            _logger.LogErrorEx("Invalid request");
            // If data is not valid, prevent further processing
            return BadRequest("Invalid request");
        }
        
        // Asynchronous method that attempts to sign in user with email and password
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false,
            lockoutOnFailure: false);

        // Log user in if email and password combination are valid
        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(user);
            // Generating token
            var token = GenerateJwtToken(user, roles);
            _logger.LogInformationEx($"Login for {model.Email} was successful");
            // Return JWT Token for user authentication
            return Ok(new { Token = token });
        }

        _logger.LogErrorEx($"Unauthorized login attempt for {model.Email}");
        return Unauthorized($"Unauthorized login attempt for {model.Email}");
    }
    
    // Log user out of their account
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformationEx("Logged out");
        return Ok("Logged out");
    }

    // Generate JSON Web Token for user authentication
    private string GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}