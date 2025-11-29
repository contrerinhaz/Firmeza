namespace Firmeza.Api.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public decimal Vat { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public List<SaleDetailDto>? SaleDetails { get; set; }
}

public class SaleDetailDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class CreateSaleDto
{
    public required string UserName { get; set; }
    public required List<CreateSaleDetailDto> Details { get; set; }
}

public class CreateSaleDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
