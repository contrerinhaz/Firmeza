namespace Firmeza.Web.Interfaces;

using Models.Entities;

public interface IProductRepository 
{
    Task<List<Product>> GetAll();
    Task<Product?> GetById(int id);
    Task<bool> Create(Product product);
    Task<bool> Update(Product product);
    Task<bool> Delete(int id);
    Task<IQueryable<Product>> GetQueryable();
    
}
