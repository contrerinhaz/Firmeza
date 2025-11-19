namespace Firmeza.Web.Interfaces;

using Models.Entities;

public interface IProductService
{
    Task<List<Product>>GetAllProducts();
    Task<Product?>GetProductById(int id);
    Task<bool>CreateProduct(Product product);
    Task<bool>UpdateProduct(Product product);
    Task<bool>DeleteProduct(int id);
}
