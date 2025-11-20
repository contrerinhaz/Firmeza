namespace Firmeza.Web.ViewModels.Products;

using System.ComponentModel.DataAnnotations;

public class EditProductViewModel : CreateProductViewModel
{
    [Required]
    public int Id { get; set; }
}
