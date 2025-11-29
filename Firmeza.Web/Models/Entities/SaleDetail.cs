namespace Firmeza.Web.Models.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SaleDetail
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Sale reference is required.")]
    [ForeignKey("Sale")]
    public int SaleId { get; set; }

    public Sale Sale { get; set; }

    [Required(ErrorMessage = "Product reference is required.")]
    [ForeignKey("Product")]
    public int ProductId { get; set; }

    public Product Product { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Unit price is required.")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999999999.99, ErrorMessage = "Unit price must be greater than 0.")]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal Subtotal => Quantity * UnitPrice;

}
