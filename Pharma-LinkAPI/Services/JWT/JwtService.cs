using Microsoft.IdentityModel.Tokens;
using Pharma_LinkAPI.DTO;
using Pharma_LinkAPI.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pharma_LinkAPI.Services.JWT
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthentcationResponse CreateToken(AppUser user)
        {
            var expiration = DateTime.UtcNow.AddDays(7);

            Claim[] claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()), // user ID
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), // token ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // issued at
                new Claim(JwtRegisteredClaimNames.Email,user.Email), // user email
                new Claim(ClaimTypes.NameIdentifier,user.UserName), // user name
                new Claim(ClaimTypes.Name,user.Name), // user full name
                new Claim(ClaimTypes.Role,user.Role) // user role
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new AuthentcationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Expiration = expiration
            };

        }
    }
}
