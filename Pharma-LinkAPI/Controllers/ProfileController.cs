﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.DTO.ProfileDTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Repositries.Irepositry;


namespace Pharma_LinkAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountRepositry _accountRepositry;
        private readonly UserManager<AppUser> _userManager;
        public ProfileController(IAccountRepositry accountRepositry, UserManager<AppUser> userManager)
        {
            _accountRepositry = accountRepositry;
            _userManager = userManager;
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
                var profile = new PharmacyProfileDTO
                {
                    Id = user.Id,
                    PharmacyName = user.Name,
                    DrName = user.DrName,
                    Street = user.Street,
                    State = user.State,
                    City = user.City,
                    PharmacyPhoneNumber = user.PhoneNumber,
                    PharmacyEmail = user.Email,
                    PharmacyLicenseNumber = user.LiscnceNumber,
                    PharmacyImagePath = user.ImagePath,
                    Role = user.Role,
                    AboutUs = user.AboutUs

                };
                return Ok(profile);
            }
            else if (user.Role == SD.Role_Company)
            {
                var companyProfile = new CompanyProfileDTO
                {
                    Id = user.Id,
                    CompanyName = user.Name,
                    Street = user.Street,
                    State = user.State,
                    City = user.City,
                    CompanyPhoneNumber = user.PhoneNumber,
                    CompanyEmail = user.Email,
                    CompanyLicenseNumber = user.LiscnceNumber,
                    CompanyImagePath = user.ImagePath,
                    Role = user.Role,
                    MinPriceToOrder = user.MinPriceToMakeOrder,
                    AboutUs = user.AboutUs
                };
                float totalRating = 0;
                if (user.ReviewsReceived == null || user.ReviewsReceived.Count == 0)
                {
                    companyProfile.CompanyRating = 0;
                    companyProfile.CurrentUserReview = 0;
                    companyProfile.TotalReviws = 0;
                    return Ok(companyProfile);
                }

                companyProfile.CurrentUserReview = 0;
                int count = 0;
                foreach (var review in user.ReviewsReceived)
                {
                    totalRating += review.Rating;
                    var pharmacy = await _accountRepositry.GetUserById(review.PharmacyId.Value);
                    if (review.Rating != 0)
                    {
                        count++;
                    }
                    companyProfile.Reviews.Add(new ReviewsDTO
                    {
                        Id = review.Id,
                        ReviewerName = pharmacy.UserName,
                        Rating = review.Rating,
                        Comment = review.Comment
                    });
                    if (pharmacy == curr)
                    {
                        companyProfile.CurrentUserReview = review.Rating;
                    }
                }
                if (count == 0)
                {
                    companyProfile.CompanyRating = 0;
                }
                else
                {
                    companyProfile.CompanyRating = (float)Math.Round(totalRating / count, 1);
                }
                companyProfile.TotalReviws = count;


                return Ok(companyProfile);
            }
            else if (user.Role == SD.Role_Admin)
            {
                return Ok(new { user.Name, user.Email, user.UserName });
            }
            return BadRequest("Invalid role");
        }


        [Authorize]
        [HttpPatch("uploadPhoto")]
        public async Task<IActionResult> uploadPhoto(IFormFile? img)
        {
            var user = await _accountRepositry.GetCurrentUser(User);
            if (user == null)
            {
                return NotFound("User not found");
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

            var uniqueFileName = $"{Guid.NewGuid()}{imgExtension}";
            var imgPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(imgPath, FileMode.Create))
            {
                img.CopyTo(stream);
            }
            #endregion

            user.ImagePath = imgPath;
            await _accountRepositry.UpdateUser(user);
            return NoContent();
        }

        [Authorize]
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassDTO changePasswordDTO)
        {
            var user = await _accountRepositry.GetUserByuserName(changePasswordDTO.username);
            var current = await _accountRepositry.GetCurrentUser(User);


            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (user.UserName != current.UserName)
            {
                return BadRequest("You can't change another user's password.");
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

        [Authorize]
        [HttpPatch("ChangePhoneNumber")]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneDTO changePhoneDTO)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var user = await _accountRepositry.GetUserByuserName(changePhoneDTO.username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var current = await _accountRepositry.GetCurrentUser(User);
            if (user.UserName != current.UserName)
            {
                return BadRequest("You can't change another user's phone number.");
            }
            user.PhoneNumber = changePhoneDTO.NewPhone;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("Phone number changed successfully.");
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

        [Authorize]
        [HttpPatch("ChangeMinPrice")]
        public async Task<IActionResult> ChangeMinPrice(ChangeMinPriceDTO dto)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var user = await _accountRepositry.GetUserByuserName(dto.username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var current = await _accountRepositry.GetCurrentUser(User);
            if (user.UserName != current.UserName)
            {
                return BadRequest("You can't change another user's min price.");
            }
            user.MinPriceToMakeOrder = dto.MinPriceToOrder;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("Min price changed successfully.");
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

        [Authorize]
        [HttpPatch("ChangeAboutUs")]
        public async Task<IActionResult> ChangeAboutUs(ChangeAboutUsDTO dto)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var user = await _accountRepositry.GetCurrentUser(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            user.AboutUs = dto.aboutUs;
            await _accountRepositry.UpdateUser(user);
            return NoContent();
        }


    }
}
