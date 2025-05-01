using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.ViewModels;
using System.Security.Claims;


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


        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            var user = await _accountRepositry.GetUserByuserName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (user.Role == SD.Role_Pharmacy)
            {
                var profile = new PharmacyProfileViewModel
                {
                    PharmacyName = user.Name,
                    DrName = user.DrName,
                    PharmacyAddress = user.Address,
                    PharmacyPhoneNumber = user.PhoneNumber,
                    PharmacyEmail = user.Email,
                    PharmacyLicenseNumber = user.LiscnceNumber,
                    PharmacyImagePath = user.ImagePath,
                    Role = user.Role
                };
                return Ok(profile);
            }
            else if (user.Role == SD.Role_Company)
            {
                var companyProfile = new CompanyProfileViewModel
                {
                    CompanyName = user.Name,
                    CompanyAddress = user.Address,
                    CompanyPhoneNumber = user.PhoneNumber,
                    CompanyEmail = user.Email,
                    CompanyLicenseNumber = user.LiscnceNumber,
                    CompanyImagePath = user.ImagePath,
                    Role = user.Role
                };
                float totalRating = 0;
                if (user.ReviewsReceived == null || user.ReviewsReceived.Count == 0)
                {
                    companyProfile.CompanyRating = 0;
                    return Ok(companyProfile);
                }



                companyProfile.Reviews = new List<ReviewViewModel>();
                foreach (var review in user.ReviewsReceived)
                {
                    totalRating += review.Rating;
                    companyProfile.Reviews.Add(new ReviewViewModel
                    {
                        Id = review.Id,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        ReviewerName = review.pharmacy?.UserName
                    });
                }
                companyProfile.CompanyRating = totalRating / user.ReviewsReceived.Count;

                return Ok(companyProfile);
            }
            else if (user.Role == SD.Role_Admin)
            {

                return Ok(new { user.Name, user.Email, user.UserName });
            }
            return BadRequest("Invalid role");
        }

        [HttpGet("me")]
        [Authorize(Roles = SD.Role_Company)]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new { userId });
        }
    }
}
