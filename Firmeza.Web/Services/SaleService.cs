using Firmeza.Web.Interfaces;
using Firmeza.Web.Models;
using Firmeza.Web.Models.Entities;

namespace Firmeza.Web.Services;

// Service responsible for handling sale business logic
public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPdfService _pdfService;

    public SaleService(ISaleRepository saleRepository, IProductRepository productRepository, IPdfService pdfService)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _pdfService = pdfService;
    }

    // Creates a new sale transaction with stock deduction
    public async Task CreateSaleAsync(Sale sale)
    {
        if (sale.SaleDetails == null || !sale.SaleDetails.Any())
        {
            throw new ArgumentException("La venta debe tener al menos un producto.");
        }

        // Validate and deduct stock for each product
        foreach (var detail in sale.SaleDetails)
        {
            var product = await _productRepository.GetById(detail.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Producto con ID {detail.ProductId} no encontrado.");
            }

            if (product.Quantity < detail.Quantity)
            {
                throw new InvalidOperationException($"Stock insuficiente para el producto '{product.Name}'. Disponible: {product.Quantity}, Solicitado: {detail.Quantity}");
            }

            product.Quantity -= detail.Quantity;
            // Product is tracked by context, changes will be saved when Sale is added
            // await _productRepository.Update(product);

            // Set unit price from product to ensure accuracy (optional, but good practice)
            if (product.Price <= 0)
            {
                throw new InvalidOperationException($"El producto '{product.Name}' tiene un precio inválido ({product.Price}). No se puede vender.");
            }
            detail.UnitPrice = product.Price;
        }

        // Calculate total amount and VAT
        sale.Total = sale.SaleDetails.Sum(d => d.Quantity * d.UnitPrice);
        sale.Vat = sale.Total * 0.19m; // VAT rate 19%
        sale.Date = DateTime.UtcNow;

        await _saleRepository.AddAsync(sale);

        // Generate PDF receipt after successful save
        await _pdfService.GenerateSaleReceiptAsync(sale);
    }

    public async Task<IEnumerable<Sale>> GetAllSalesAsync()
    {
        return await _saleRepository.GetAllAsync();
    }

    public async Task<Sale?> GetSaleByIdAsync(int id)
    {
        return await _saleRepository.GetByIdAsync(id);
    }

    // Retrieves paginated sales
    public async Task<PaginatedList<Sale>> GetPagedSalesAsync(int pageNumber, int pageSize)
    {
        var query = await _saleRepository.GetQueryable();
        return await PaginatedList<Sale>.CreateAsync(query, pageNumber, pageSize);
    }
}
