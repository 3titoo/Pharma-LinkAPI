using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.Services.JWT;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;

        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost("Register/{Id}")]
        public async Task<ActionResult<AuthentcationResponse>> Register(int Id)
        {
            var request = _unitOfWork._requestRepositry.GetById(Id);
            if (request == null)
            {
                return NotFound("Request not found.");
            }
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var user = new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                LiscnceNumber = request.Pharmacy_License,
                Street = request.Street,
                State = request.State,
                City = request.City,
                Name = request.Pharmacy_Name,
                Role = SD.Role_Pharmacy,
                EmailConfirmed = true,
                //ImagePath = request.ImageUrl,
                DrName = request.DR_Name
            };
            string? password = request.Password;
            if (password == null || password[0] == ' ')
            {
                return BadRequest("Password is required.");
            }
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Role_Pharmacy);

                await _signInManager.SignInAsync(user, isPersistent: false);

                //add cart to user

                var cart = new Cart
                {
                    TotalPrice = 0,
                    PharmacyId = user.Id
                };
                _unitOfWork._cartRepositry.AddCart(cart);

                // Generate JWT token 

                var token = _jwtService.CreateToken(user);
                _unitOfWork._requestRepositry.Delete(Id);
                await _unitOfWork._emailService.SendEmailAsync(request.Email, "Pharmacy Account Created", $"Your request has been Accepted successfully.\n username is {request.UserName},password is {request.Password}");
                return Ok(token);
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost("CompanyRegister")]
        public async Task<ActionResult<string>> Register(CompanyRegisterDTO companyRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var existEmail = await _userManager.FindByEmailAsync(companyRegisterDTO.Email);
            if (existEmail != null)
            {
                return BadRequest("Email is already in use");
            }

            var user = new AppUser
            {
                UserName = companyRegisterDTO.UserName,
                Email = companyRegisterDTO.Email,
                PhoneNumber = companyRegisterDTO.PhoneNumber,
                LiscnceNumber = companyRegisterDTO.LicenseNumber,
                Street = companyRegisterDTO.Street,
                State = companyRegisterDTO.State,
                City = companyRegisterDTO.City,
                Name = companyRegisterDTO.Name,
                Role = SD.Role_Company,
                MinPriceToMakeOrder = companyRegisterDTO.MinPriceToMakeOrder,
                EmailConfirmed = true
            };
            string? password = companyRegisterDTO.Password;
            if (password == null || password[0] == ' ')
            {
                return BadRequest("Password is required.");
            }
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Role_Company);
                return Ok("company is created");
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            var email = await _userManager.FindByEmailAsync(loginDTO.UserName);
            if (email != null)
            {
                loginDTO.UserName = email.UserName;
            }

            // Check if the user exists
            var result = await _signInManager.PasswordSignInAsync(loginDTO.UserName, loginDTO.Password, isPersistent: loginDTO.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginDTO.UserName);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Generate JWT token 

                var token = _jwtService.CreateToken(user);

                return Ok(token);
            }
            return BadRequest("Invalid User Name or Password");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }



        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userName)
        {

            if (userName == "admin")
            {
                return BadRequest("You can't delete the admin account.");
            }
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("User name is required.");
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (user.pdfPath != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), user.pdfPath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            if (user.ImagePath != null)
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), user.ImagePath);
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }

            if (user.Role == SD.Role_Pharmacy)
            {
                var reviews = _unitOfWork._reviewRepositiry.GetReviewsByPharmacyId(user.Id);

                if (reviews != null)
                {
                    foreach (var review in reviews)
                    {
                        _unitOfWork._reviewRepositiry.Delete(review.Id);
                    }
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("User deleted successfully.");
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

    }
}
