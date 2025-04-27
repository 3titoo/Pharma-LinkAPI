using Pharma_LinkAPI.Identity;
using System.Security.Claims;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface IAccountRepositry
    {

        Task<AppUser?> GetUserByuserName(string userName);
        Task<AppUser?> GetUserByEmail(string email);
        Task<AppUser?> GetUserById(int id);
        Task<AppUser?> GetCurrentUser(ClaimsPrincipal user);

        Task<AppUser?> GetCompanyByEmailWithReviews(string email);
        Task<IEnumerable<AppUser?>> GetAllUsers(string role = SD.Role_Pharmacy);


    }
}
