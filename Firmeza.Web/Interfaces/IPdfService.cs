using Firmeza.Web.Models.Entities;

namespace Firmeza.Web.Interfaces;

public interface IPdfService
{
    byte[] GenerateProductListPdf(IEnumerable<Product> products);
    byte[] GenerateClientListPdf(IEnumerable<User> clients);
    byte[] GenerateSaleListPdf(IEnumerable<Sale> sales);
    Task<string> GenerateSaleReceiptAsync(Sale sale);
}
