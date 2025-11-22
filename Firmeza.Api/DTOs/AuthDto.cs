using System.ComponentModel.DataAnnotations;

namespace Firmeza.Api.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}

public class RegisterDto
{
    [Required]
    public required string FullName { get; set; }

    [Required]
    public required string DocumentNumber { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Phone { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}
