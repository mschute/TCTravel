using System.ComponentModel.DataAnnotations;

namespace TCTravel.Models;

public class AuthModel
{
    // Added EmailAddress attribute to perform basic email format validation
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}