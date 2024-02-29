using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TCTravel.Models;
using TCTravel.Services;

namespace TCTravel.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly EmailService _emailService;

    public AccountController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, EmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthModel model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            // Generate an email verification token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Create the verification link
            var verificationLink = Url.Action("VerifyEmail", "Account", new
            {
                userId = user.Id, token =
                    token
            }, Request.Scheme);
            // Send the verification email
            var emailSubject = "Email Verification";
            var emailBody = $"Please verify your email by clicking the following link: {verificationLink}";
            _emailService.SendEmail(user.Email, emailSubject, emailBody);

            return Ok("User registered successfully. An email verification link has been sent.");
        }

        return BadRequest(result.Errors);
    }

    // Add an action to handle email verification
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return Ok("Email verification successful.");
        }

        return BadRequest("Email verification failed.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,
            isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return Ok("Login successful.");
        }

        return Unauthorized("Invalid login attempt.");
    }
}