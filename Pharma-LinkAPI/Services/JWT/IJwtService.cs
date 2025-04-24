using Pharma_LinkAPI.DTO;
using Pharma_LinkAPI.Identity;

namespace Pharma_LinkAPI.Services.JWT
{
    public interface IJwtService
    {
        AuthentcationResponse CreateToken(AppUser user);
    }
}
