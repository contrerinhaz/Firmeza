using Firmeza.Web.Interfaces;
using Firmeza.Web.Models;
using Firmeza.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Web.Services
{
    // Service for managing user accounts and roles
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            IUserRepository repo,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _repo = repo;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task<List<User>> GetAllAsync() => _repo.GetAllAsync();

        public Task<User?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);

        public Task CreateAsync(User user) => _repo.AddAsync(user);

        public Task UpdateAsync(User user) => _repo.UpdateAsync(user);

        public async Task DeleteAsync(string id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user != null)
                await _repo.DeleteAsync(user);
        }

        public Task<bool> ExistsAsync(string id) => _repo.ExistsAsync(id);

        // Creates a new user with password and assigns a role
        public async Task CreateWithPasswordAsync(User user, string password, string role)
        {
            // Crear usuario con Identity
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ",
                result.Errors.Select(e => e.Description)));

            // Rol
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);
        }

        // Retrieves paginated users
        public async Task<PaginatedList<User>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = await _repo.GetQueryable();
            return await PaginatedList<User>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}
