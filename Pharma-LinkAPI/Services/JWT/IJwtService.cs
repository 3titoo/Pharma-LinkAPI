using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.Identity;

namespace Pharma_LinkAPI.Services.JWT
{
    public interface IJwtService
    {
        AuthentcationResponse CreateToken(AppUser user);
    }
}
