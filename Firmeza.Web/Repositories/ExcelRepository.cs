using Firmeza.Web.Data;
using Firmeza.Web.DTOs;
using Firmeza.Web.Interfaces;
using Firmeza.Web.Models.Entities;

namespace Firmeza.Web.Repositories;

public class ExcelRepository(AppDbContext context) : IExcelRepository
{
    public async Task SaveProductsFromExcelAsync(IEnumerable<ExcelProductDto> excelProducts)
    {
        var products = excelProducts.Select(dto => new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await context.Product.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}