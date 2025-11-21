namespace Firmeza.Web.Controllers;

using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Firmeza.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Data;

[Microsoft.AspNetCore.Components.Route("[controller]/[action]")]

// Controller for Excel file import/export operations
public class ExcelController : Controller
{
    private readonly IExcelService _excelService;
    private readonly AppDbContext _context;

    public ExcelController(IExcelService excelService, AppDbContext context)
    {
        _excelService = excelService;
        _context = context;
    }

    // Displays the upload view
    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    // Processes the uploaded Excel file
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Por favor selecciona un archivo Excel válido.";
            return RedirectToAction("Upload");
        }

        var success = await _excelService.ProcessExcelAsync(file);

        TempData["Message"] = success
            ? "Datos cargados correctamente a la base de datos."
            : "Ocurrió un error al procesar el archivo.";

        return RedirectToAction("Upload");
    }

    // Exports the list of products to an Excel file
    [HttpGet]
    public async Task<IActionResult> ExportProducts()
    {
        var products = await _context.Products.ToListAsync();
        var fileContent = _excelService.ExportProducts(products);
        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Productos.xlsx");
    }

    // Exports the list of clients to an Excel file
    [HttpGet]
    public async Task<IActionResult> ExportClients()
    {
        var clients = await _context.Users.ToListAsync();
        var fileContent = _excelService.ExportClients(clients);
        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Clientes.xlsx");
    }

    // Exports the list of sales to an Excel file
    [HttpGet]
    public async Task<IActionResult> ExportSales()
    {
        var sales = await _context.Sales.Include(s => s.User).ToListAsync();
        var fileContent = _excelService.ExportSales(sales);
        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Ventas.xlsx");
    }
}
