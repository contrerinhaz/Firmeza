using Firmeza.Web.Data;
using Firmeza.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Controllers;

// Controller for generating PDF reports
public class ReportsController : Controller
{
    private readonly IPdfService _pdfService;
    private readonly AppDbContext _context;

    public ReportsController(IPdfService pdfService, AppDbContext context)
    {
        _pdfService = pdfService;
        _context = context;
    }

    // Generates and downloads the product list PDF
    [HttpGet]
    public async Task<IActionResult> ExportProductsPdf()
    {
        var products = await _context.Products.ToListAsync();
        var pdfBytes = _pdfService.GenerateProductListPdf(products);
        return File(pdfBytes, "application/pdf", "Reporte_Productos.pdf");
    }

    // Generates and downloads the client list PDF
    [HttpGet]
    public async Task<IActionResult> ExportClientsPdf()
    {
        var clients = await _context.Users.ToListAsync();
        var pdfBytes = _pdfService.GenerateClientListPdf(clients);
        return File(pdfBytes, "application/pdf", "Reporte_Clientes.pdf");
    }

    // Generates and downloads the sales list PDF
    [HttpGet]
    public async Task<IActionResult> ExportSalesPdf()
    {
        var sales = await _context.Sales
            .Include(s => s.User)
            .Include(s => s.SaleDetails!)
            .ThenInclude(sd => sd.Product)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
        var pdfBytes = _pdfService.GenerateSaleListPdf(sales);
        return File(pdfBytes, "application/pdf", "Reporte_Ventas.pdf");
    }
}
