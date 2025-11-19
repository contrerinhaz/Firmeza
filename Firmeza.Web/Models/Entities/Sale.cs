namespace Firmeza.Web.Models.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Sale
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Sale date is required.")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Total amount is required.")]
    [Column(TypeName = "decimal(10,2)")]
    [Range(0.01, 999999.99, ErrorMessage = "Total must be greater than 0.")]
    public decimal Total { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 999999.99, ErrorMessage = "VAT value must be valid.")]
    public decimal Vat { get; set; }

    [ForeignKey("User")]
    public string? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<SaleDetail>? SaleDetails { get; set; }  
}
