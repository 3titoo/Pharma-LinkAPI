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

        [HttpGet("company/{username}")]
        public async Task<ActionResult<CompanyProfileViewModel>> GetCompanyProfile(string username)
        {
            var company = await _accountRepositry.GetCompanyByEmailWithReviews(username);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            var profile = new CompanyProfileViewModel
            {
                CompanyName = company.Name,
                CompanyAddress = company.Address,
                CompanyPhoneNumber = company.PhoneNumber,
                CompanyEmail = company.Email,
                CompanyLicenseNumber = company.LiscnceNumber,
                CompanyImagePath = company.ImagePath,
            };
            float totalRating = 0;
            if(company.ReviewsReceived == null || company.ReviewsReceived.Count == 0)
            {
                profile.CompanyRating = 0;
                return Ok(profile);
            }



            profile.Reviews = new List<ReviewViewModel>();
            foreach (var review in company.ReviewsReceived)
            {
                totalRating += review.Rating;
                profile.Reviews.Add(new ReviewViewModel
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    ReviewerName = review.pharmacy?.UserName
                });
            }
            profile.CompanyRating = totalRating / company.ReviewsReceived.Count;

            return Ok(profile);
        }




    }
}
