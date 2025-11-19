namespace Firmeza.Web.Repositories;

using Data;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

public class ProductRepository : IProductRepository 
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAll()
    {
        try
        {
            return await _context.Product.ToListAsync();
        }
        catch (Exception ex)
        {
            return [];
        }
    }
    
    public async Task<Product?> GetById(int id)
    {
        try
        {
            return await _context.Product.FindAsync(id);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    
    public async Task<bool> Create(Product product)
    {
        try
        {
            await _context.Product.AddAsync(product);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch (Exception ex)
        {
            return false;
        }    
    }
    public async Task<bool> Update(Product product)
    {
        try
        {
            _context.Product.Update(product);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<bool> Delete(int id)
    {
        try
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return false;
            }
            _context.Product.Remove(product);
            var results = await _context.SaveChangesAsync();
            return results > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
        
    }
}
