using Firmeza.Web.DTOs;
using Firmeza.Web.Interfaces;
using OfficeOpenXml;

namespace Firmeza.Web.Services;

public class ExcelService : IExcelService
{
    private readonly IExcelRepository _excelRepository;

    public ExcelService(IExcelRepository excelRepository)
    {
        _excelRepository = excelRepository;
    }

    public async Task<bool> ProcessExcelAsync(IFormFile file)
    {
        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null) return false;

            var products = new List<ExcelProductDto>();
            int totalRows = worksheet.Dimension.End.Row;

            for (int row = 2; row <= totalRows; row++)
            {
                if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text))
                    continue;

                products.Add(new ExcelProductDto
                {
                    Name = worksheet.Cells[row, 1].Text,
                    Price = decimal.TryParse(worksheet.Cells[row, 2].Value?.ToString(), 
                    System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    out var price) ? price : 0,
                    Quantity = int.TryParse(worksheet.Cells[row, 3].Text, out var qty) ? qty : 0,
                    Category = worksheet.Cells[row, 4].Text
                });
            }

            await _excelRepository.SaveProductsFromExcelAsync(products);
            return true;
        }
        catch
        {
            return false;
        }
    }
}