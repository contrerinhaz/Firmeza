using Firmeza.Web.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Firmeza.Web.Interfaces
{
    public interface IExcelService
    {
        Task<bool> ProcessExcelAsync(IFormFile file);
        byte[] ExportProducts(IEnumerable<Product> products);
        byte[] ExportClients(IEnumerable<User> clients);
        byte[] ExportSales(IEnumerable<Sale> sales);
    }
}