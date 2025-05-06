using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.ViewModels;
using System.Security.Claims;


namespace Pharma_LinkAPI.Controllers
{
    //[Authorize]
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
            var curr = await _accountRepositry.GetCurrentUser(User);
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
                    Street = user.Street,
                    State = user.State,
                    City = user.City,
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
                    Street = user.Street,
                    State = user.State,
                    City = user.City,
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

                companyProfile.CurrentUserReview = 0;
                foreach (var review in user.ReviewsReceived)
                {
                    totalRating += review.Rating;
                    var pharmacy = await _accountRepositry.GetUserById(review.PharmacyId.Value);
                    if (pharmacy == curr) {
                        companyProfile.CurrentUserReview = review.Rating;
                    }
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



        [HttpPut("{id}")]
        public async Task<IActionResult> uploadPhoto(int id,IFormFile? img)
        {
            var user = await _accountRepositry.GetCurrentUser(User);
            var curr = await _accountRepositry.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (user != curr)
            {
                return BadRequest("You are not authorized to update this profile");
            }
            if (img == null || img.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            #region Image
            if (img == null || img.Length == 0)
                return BadRequest("No image uploaded.");
            var imgExtension = Path.GetExtension(img.FileName).ToLower();
            if (imgExtension != ".jpg" && imgExtension != ".png" && imgExtension != ".jpeg")
                return BadRequest("Only JPG, PNG, and JPEG files are allowed.");
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);
            var imgPath = Path.Combine(uploadsFolder, img.FileName);
            using (var stream = new FileStream(imgPath, FileMode.Create))
            {
                img.CopyTo(stream);
            }
            #endregion

            user.ImagePath = imgPath;
            _accountRepositry.UpdateUser(user);
            return NoContent();
        }

    }
}
