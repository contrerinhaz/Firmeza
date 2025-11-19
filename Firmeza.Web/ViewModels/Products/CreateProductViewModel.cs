namespace Firmeza.Web.ViewModels.Products;

using System.ComponentModel.DataAnnotations;

public class CreateProductViewModel {
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-]+$", ErrorMessage = "Product name can only contain letters, numbers, spaces, and hyphens.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
    public int Quantity { get; set; }

    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-]+$", ErrorMessage = "Product name can only contain letters, numbers, spaces, and hyphens.")]
    public string? Category { get; set; }
}
