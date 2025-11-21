using Firmeza.Web.DTOs;
using Firmeza.Web.Interfaces;
using Firmeza.Web.Models.Entities;
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

    public byte[] ExportProducts(IEnumerable<Product> products)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Productos");

        worksheet.Cells[1, 1].Value = "Nombre";
        worksheet.Cells[1, 2].Value = "Categoría";
        worksheet.Cells[1, 3].Value = "Precio";
        worksheet.Cells[1, 4].Value = "Stock";

        using (var range = worksheet.Cells[1, 1, 1, 4])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        int row = 2;
        foreach (var product in products)
        {
            worksheet.Cells[row, 1].Value = product.Name;
            worksheet.Cells[row, 2].Value = product.Category;
            worksheet.Cells[row, 3].Value = product.Price;
            worksheet.Cells[row, 4].Value = product.Quantity;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public byte[] ExportClients(IEnumerable<User> clients)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Clientes");

        worksheet.Cells[1, 1].Value = "Nombre";
        worksheet.Cells[1, 2].Value = "Email";
        worksheet.Cells[1, 3].Value = "Teléfono";
        worksheet.Cells[1, 4].Value = "Documento";

        using (var range = worksheet.Cells[1, 1, 1, 4])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        int row = 2;
        foreach (var client in clients)
        {
            worksheet.Cells[row, 1].Value = client.FullName;
            worksheet.Cells[row, 2].Value = client.Email;
            worksheet.Cells[row, 3].Value = client.Phone;
            worksheet.Cells[row, 4].Value = client.DocumentNumber;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public byte[] ExportSales(IEnumerable<Sale> sales)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Ventas");

        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Fecha";
        worksheet.Cells[1, 3].Value = "Cliente";
        worksheet.Cells[1, 4].Value = "Total";
        worksheet.Cells[1, 5].Value = "IVA";

        using (var range = worksheet.Cells[1, 1, 1, 5])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        int row = 2;
        foreach (var sale in sales)
        {
            worksheet.Cells[row, 1].Value = sale.Id;
            worksheet.Cells[row, 2].Value = sale.Date.ToString("g");
            worksheet.Cells[row, 3].Value = sale.User?.FullName ?? "N/A";
            worksheet.Cells[row, 4].Value = sale.Total;
            worksheet.Cells[row, 5].Value = sale.Vat;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}