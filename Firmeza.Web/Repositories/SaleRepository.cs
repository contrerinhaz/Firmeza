using Firmeza.Web.Data;
using Firmeza.Web.Interfaces;
using Firmeza.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Repositories;

// Repository for accessing Sale data in the database
public class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _context;

    public SaleRepository(AppDbContext context)
    {
        _context = context;
    }

    // Adds a new sale and saves changes to the database
    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.User)
            .Include(s => s.SaleDetails!)
            .ThenInclude(sd => sd.Product)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.User)
            .Include(s => s.SaleDetails!)
            .ThenInclude(sd => sd.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    // Returns queryable for pagination
    public async Task<IQueryable<Sale>> GetQueryable()
    {
        return _context.Sales
            .Include(s => s.User)
            .Include(s => s.SaleDetails!)
            .ThenInclude(sd => sd.Product)
            .OrderByDescending(s => s.Date);
    }
}
