namespace Firmeza.Web.Controllers;

using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

// Controller for managing sale details (CRUD operations)
public class SaleDetailsController : Controller
{
    private readonly AppDbContext _context;

    public SaleDetailsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: SaleDetails
    // Lists all sale details
    public async Task<IActionResult> Index()
    {
        var appDbContext = _context.SaleDetails.Include(s => s.Product).Include(s => s.Sale);
        return View(await appDbContext.ToListAsync());
    }

    // GET: SaleDetails/Details/5
    // Shows details of a specific sale item
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var saleDetail = await _context.SaleDetails
            .Include(s => s.Product)
            .Include(s => s.Sale)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (saleDetail == null)
        {
            return NotFound();
        }

        return View(saleDetail);
    }

    // GET: SaleDetails/Create
    // Prepares view for creating a new sale detail
    public IActionResult Create()
    {
        ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name");
        ViewData["SaleId"] = new SelectList(_context.Set<Sale>(), "Id", "Id");
        return View();
    }

    // POST: SaleDetails/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Processes the creation of a new sale detail
    public async Task<IActionResult> Create([Bind("Id,SaleId,ProductId,Quantity,UnitPrice")] SaleDetail saleDetail)
    {
        if (ModelState.IsValid)
        {
            _context.Add(saleDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", saleDetail.ProductId);
        ViewData["SaleId"] = new SelectList(_context.Set<Sale>(), "Id", "Id", saleDetail.SaleId);
        return View(saleDetail);
    }

    // GET: SaleDetails/Edit/5
    // Prepares view for editing an existing sale detail
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var saleDetail = await _context.SaleDetails.FindAsync(id);
        if (saleDetail == null)
        {
            return NotFound();
        }
        ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", saleDetail.ProductId);
        ViewData["SaleId"] = new SelectList(_context.Set<Sale>(), "Id", "Id", saleDetail.SaleId);
        return View(saleDetail);
    }

    // POST: SaleDetails/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Processes the update of a sale detail
    public async Task<IActionResult> Edit(int id, [Bind("Id,SaleId,ProductId,Quantity,UnitPrice")] SaleDetail saleDetail)
    {
        if (id != saleDetail.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(saleDetail);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleDetailExists(saleDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", saleDetail.ProductId);
        ViewData["SaleId"] = new SelectList(_context.Set<Sale>(), "Id", "Id", saleDetail.SaleId);
        return View(saleDetail);
    }

    // GET: SaleDetails/Delete/5
    // Prepares view for deleting a sale detail
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var saleDetail = await _context.SaleDetails
            .Include(s => s.Product)
            .Include(s => s.Sale)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (saleDetail == null)
        {
            return NotFound();
        }

        return View(saleDetail);
    }

    // POST: SaleDetails/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    // Confirms and executes the deletion of a sale detail
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var saleDetail = await _context.SaleDetails.FindAsync(id);
        if (saleDetail != null)
        {
            _context.SaleDetails.Remove(saleDetail);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SaleDetailExists(int id)
    {
        return _context.SaleDetails.Any(e => e.Id == id);
    }
}

