using System.ComponentModel.DataAnnotations;

namespace TCTravel.Models;

public class AuthModel
{
    // EmailAddress attribute performs basic email format validation
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}