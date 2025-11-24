using Firmeza.Web.Models;
using Firmeza.Web.Models.Entities;

namespace Firmeza.Web.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task CreateWithPasswordAsync(User user, string password, string role);
        Task<PaginatedList<User>> GetPagedAsync(int pageNumber, int pageSize);
    }
}
