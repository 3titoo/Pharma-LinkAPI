using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Repositries.Irepositry;
using System.Security.Claims;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class AccountRepo : IAccountRepositry
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;

        public AccountRepo(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<AppUser?> GetCurrentUser(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return null;
            }
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                return null;
            }
            var userWithReviews = await _userManager.Users
            .Include(u => u.ReviewsGiven)
            .Include(u => u.ReviewsReceived)
            .Include(c => c.Cart)
            .FirstOrDefaultAsync(u => u.Id == appUser.Id);
            return userWithReviews;
        }
        public async Task<AppUser?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            var userWithReviews = await _userManager.Users
            .Include(u => u.ReviewsGiven)
            .Include(u => u.ReviewsReceived)
            .FirstOrDefaultAsync(u => u.Id == user.Id);
            return userWithReviews;
        }
        public async Task<AppUser?> GetUserById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return null;
            }
            var userWithReviews = await _userManager.Users
            .Include(u => u.ReviewsGiven)
            .Include(u => u.ReviewsReceived)
            .FirstOrDefaultAsync(u => u.Id == user.Id);
            return userWithReviews;

        }
        public async Task<AppUser?> GetUserByuserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return null;
            }

            var userWithReviews = await _userManager.Users
                .Include(u => u.ReviewsGiven)
                .Include(u => u.ReviewsReceived)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            return userWithReviews;
        }

        public async Task<IEnumerable<AppUser?>> GetAllUsers(string role = SD.Role_Pharmacy)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            if (users == null)
            {
                return null;
            }
            return users;
        }

        public async Task<AppUser?> GetCompanyByEmailWithReviews(string email)
        {
            var company = await _userManager.Users
                .Include(c => c.ReviewsReceived)
                .FirstOrDefaultAsync(c => c.Email == email);
            if (company == null)
            {
                return null;
            }
            return company;
        }

        public async Task UpdateUser(AppUser user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Street = user.Street;
                existingUser.City = user.City;
                existingUser.State = user.State;
                existingUser.ImagePath = user.ImagePath;
                await _userManager.UpdateAsync(existingUser);
                await _dbContext.SaveChangesAsync();

            }
        }
    }
}
