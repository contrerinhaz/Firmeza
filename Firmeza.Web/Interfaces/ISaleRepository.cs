using Firmeza.Web.Models.Entities;

namespace Firmeza.Web.Interfaces;

public interface ISaleRepository
{
    Task AddAsync(Sale sale);
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(int id);
    Task<IQueryable<Sale>> GetQueryable();
}
