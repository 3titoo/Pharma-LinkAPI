using Microsoft.AspNetCore.Identity;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Repositries.Irepositry;
using System.Security.Claims;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class AccountRepo : IAccountRepositry
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountRepo(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
            return appUser;
        }
        public async Task<AppUser?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        public async Task<AppUser?> GetUserById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return null;
            }
            return user;

        }
        public async Task<AppUser?> GetUserByuserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return null;
            }
            return user;
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

    }
}
