using Firmeza.Web.Data;
using Firmeza.Web.Interfaces;
using Firmeza.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Repositories
{
    // Repository for User entity database operations
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all users from the database
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Finds a user by their unique ID
        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Adds a new user to the database
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Updates an existing user's information
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Removes a user from the database
        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<User?> GetByNameAsync(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.FullName.ToLower() == name.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Returns queryable for pagination
        public async Task<IQueryable<User>> GetQueryable()
        {
            return _context.Users.OrderBy(u => u.FullName);
        }
    }
}
