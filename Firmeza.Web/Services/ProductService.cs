namespace Firmeza.Web.Services;

using Interfaces;
using Models;
using Models.Entities;

// Service for managing product business logic
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // Retrieves all products from the repository
    public async Task<List<Product>> GetAllProducts()
    {
        return await _productRepository.GetAll();
    }

    // Retrieves a single product by its unique identifier
    public async Task<Product?> GetProductById(int id)
    {
        if (id <= 0) return null;

        return await _productRepository.GetById(id);
    }

    // Validates and creates a new product
    public Task<bool> CreateProduct(Product product)
    {
        if (product == null ||
            string.IsNullOrWhiteSpace(product.Name) ||
            product.Price <= 0 ||
            product.Quantity < 0 ||
            (!string.IsNullOrWhiteSpace(product.Category) && product.Category.Length > 50)) return Task.FromResult(false);

        return _productRepository.Create(product);
    }

    // Validates and updates an existing product
    public Task<bool> UpdateProduct(Product product)
    {
        if (product == null ||
            string.IsNullOrWhiteSpace(product.Name) ||
            product.Price <= 0 ||
            product.Quantity < 0 ||
            (!string.IsNullOrWhiteSpace(product.Category) && product.Category.Length > 50)) return Task.FromResult(false);

        return _productRepository.Update(product);
    }

    // Deletes a product by its ID
    public Task<bool> DeleteProduct(int id)
    {
        if (id <= 0) return Task.FromResult(false);
        return _productRepository.Delete(id);
    }

    // Retrieves paginated products
    public async Task<PaginatedList<Product>> GetPagedProductsAsync(int pageNumber, int pageSize)
    {
        var query = await _productRepository.GetQueryable();
        return await PaginatedList<Product>.CreateAsync(query, pageNumber, pageSize);
    }
}
