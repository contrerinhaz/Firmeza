using Firmeza.Web.Interfaces;
using Firmeza.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firmeza.Web.Controllers;

// Controller for managing sales operations
public class SaleController : Controller
{
    private readonly ISaleService _saleService;
    private readonly IUserService _userService;
    private readonly IProductService _productService;
    private readonly IPdfService _pdfService;

    public SaleController(ISaleService saleService, IUserService userService, IProductService productService, IPdfService pdfService)
    {
        _saleService = saleService;
        _userService = userService;
        _productService = productService;
        _pdfService = pdfService;
    }

    // GET: Sale
    // Retrieves and displays a list of all sales
    public async Task<IActionResult> Index(int pageNumber = 1)
    {
        const int pageSize = 10;
        var sales = await _saleService.GetPagedSalesAsync(pageNumber, pageSize);
        return View(sales);
    }

    // GET: Sale/Details/5
    // Shows detailed information for a specific sale
    public async Task<IActionResult> Details(int id)
    {
        var sale = await _saleService.GetSaleByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }
        return View(sale);
    }

    // GET: Sale/Create
    // Prepares the view for creating a new sale
    public async Task<IActionResult> Create()
    {
        var users = await _userService.GetAllAsync();
        ViewData["UserId"] = new SelectList(users, "Id", "FullName");

        var products = await _productService.GetAllProducts();
        ViewBag.Products = products; // Pass products for JS selection

        return View();
    }

    // POST: Sale/Create
    // Processes the creation of a new sale
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] SaleViewModel saleVm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var sale = new Sale
            {
                UserId = saleVm.UserId,
                Date = DateTime.UtcNow,
                SaleDetails = saleVm.Items.Select(i => new SaleDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            await _saleService.CreateSaleAsync(sale);

            // Return the ID or redirect URL so the frontend can redirect
            return Ok(new { redirectUrl = Url.Action(nameof(Index)) });
        }
        catch (Exception ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new { message });
        }
    }

    // Downloads the PDF receipt for a sale
    public async Task<IActionResult> DownloadReceipt(int id)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recibos");
        var files = Directory.GetFiles(folderPath, $"Recibo_{id}_*.pdf");

        string filePath;
        if (files.Any())
        {
            filePath = files.OrderByDescending(f => f).First();
        }
        else
        {
            var saleFull = await _saleService.GetSaleByIdAsync(id);
            if (saleFull == null) return NotFound();

            var fileName = await _pdfService.GenerateSaleReceiptAsync(saleFull);
            filePath = Path.Combine(folderPath, fileName);
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(bytes, "application/pdf", Path.GetFileName(filePath));
    }
}

public class SaleViewModel
{
    public string UserId { get; set; }
    public List<SaleItemViewModel> Items { get; set; }
}

public class SaleItemViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
