using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.ViewModels;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountRepositry _accountRepositry;
        public ProfileController(IAccountRepositry accountRepositry)
        {
            _accountRepositry = accountRepositry;
        }

        [HttpGet("pharmacy/{username}")]
        public async Task<ActionResult<PharmacyProfileViewModel>> GetPharmacyProfile(string username)
        {
            var user = await _accountRepositry.GetUserByuserName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var profile = new PharmacyProfileViewModel
            {
                PharmacyName = user.Name,
                DrName = user.DrName,
                PharmacyAddress = user.Address,
                PharmacyPhoneNumber = user.PhoneNumber,
                PharmacyEmail = user.Email,
                PharmacyLicenseNumber = user.LiscnceNumber,
                PharmacyImagePath = user.ImagePath
            };
            return Ok(profile);
        }




    }
}
