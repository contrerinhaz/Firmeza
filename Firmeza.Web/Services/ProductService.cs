namespace Firmeza.Web.Services;

using Interfaces;
using Models.Entities;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _productRepository.GetAll();
    }
    
    public async Task<Product?> GetProductById(int id)
    {
        if (id == null) return null;
        
        return await _productRepository.GetById(id);
    }

    public Task<bool> CreateProduct(Product product)
    {
        if (product == null ||
            string.IsNullOrWhiteSpace(product.Name) ||
            product.Price <= 0 ||
            product.Quantity < 0 ||
            (!string.IsNullOrWhiteSpace(product.Category) && product.Category.Length > 50)) return Task.FromResult(false);

        return _productRepository.Create(product);
    }
    
    public Task<bool> UpdateProduct(Product product)
    {
        if (product == null ||
            string.IsNullOrWhiteSpace(product.Name) ||
            product.Price <= 0 ||
            product.Quantity < 0 ||
            (!string.IsNullOrWhiteSpace(product.Category) && product.Category.Length > 50)) return Task.FromResult(false);

        return _productRepository.Update(product);
    }
    
    public Task<bool> DeleteProduct(int id)
    {
        if (id == null) return Task.FromResult(false);
        return _productRepository.Delete(id);    
    }
}
