namespace Firmeza.Web.ViewModels.Users;

using System.ComponentModel.DataAnnotations;

public class CreateUserViewModel {
    
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-]+$", ErrorMessage = "Full name can only contain letters, numbers, spaces, and hyphens.")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Document number is required.")]
    [StringLength(20, ErrorMessage = "Document number cannot exceed 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-]+$", ErrorMessage = "Document Number can only contain letters, numbers, spaces, and hyphens.")]
    public string DocumentNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Phone number is required.")]
    [Phone]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    public string Phone { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
    
    
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-]+$", ErrorMessage = "Username can only contain letters, numbers, spaces, and hyphens.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
