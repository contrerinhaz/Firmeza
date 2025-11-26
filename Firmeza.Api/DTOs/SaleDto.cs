namespace Firmeza.Api.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public decimal Vat { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

public class CreateSaleDto
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public required List<CreateSaleDetailDto> Details { get; set; }
}

public class CreateSaleDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
