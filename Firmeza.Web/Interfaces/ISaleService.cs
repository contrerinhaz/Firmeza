using Firmeza.Web.Models;
using Firmeza.Web.Models.Entities;

namespace Firmeza.Web.Interfaces;

public interface ISaleService
{
    Task CreateSaleAsync(Sale sale);
    Task<IEnumerable<Sale>> GetAllSalesAsync();
    Task<Sale?> GetSaleByIdAsync(int id);
    Task<PaginatedList<Sale>> GetPagedSalesAsync(int pageNumber, int pageSize);
}
