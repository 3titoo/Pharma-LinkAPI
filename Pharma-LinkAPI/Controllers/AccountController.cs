using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Services.JWT;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IWebHostEnvironment env,IJwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("PhRegister")]
        public async Task<ActionResult<AuthentcationResponse>> Register(PharmacyRegisterDTO pharmacyRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var file = pharmacyRegisterDTO.file;

            #region File
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".pdf")
                return BadRequest("Only PDF files are allowed.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            #endregion

            var user = new AppUser
            {
                UserName = pharmacyRegisterDTO.UserName,
                Email = pharmacyRegisterDTO.Email,
                PhoneNumber = pharmacyRegisterDTO.PhoneNumber,
                LiscnceNumber = pharmacyRegisterDTO.LicenseNumber,
                Address = pharmacyRegisterDTO.Address,
                Name = pharmacyRegisterDTO.Name,
                pdfPath = filePath,
                Role = SD.Role_Pharmacy,
                EmailConfirmed = true
            };
            string? password = pharmacyRegisterDTO.Password;
            if(password == null || password[0] == ' ')
            {
                return BadRequest("Password is required.");
            }
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Role_Pharmacy);

                await _signInManager.SignInAsync(user, isPersistent: false);

                // Generate JWT token 

               var token = _jwtService.CreateToken(user);

                return Ok(token);
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

        [HttpPost("CoRegister")]
        public async Task<ActionResult<AuthentcationResponse>> Register(CompanyRegisterDTO companyRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            var user = new AppUser
            {
                UserName = companyRegisterDTO.UserName,
                Email = companyRegisterDTO.Email,
                PhoneNumber = companyRegisterDTO.PhoneNumber,
                LiscnceNumber = companyRegisterDTO.LicenseNumber,
                Address = companyRegisterDTO.Address,
                Name = companyRegisterDTO.Name,
                Role = SD.Role_Company,
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
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Generate JWT token 

                var token = _jwtService.CreateToken(user);

                return Ok(token);
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

        [HttpGet("Email")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Ok(true);
            }
            return Ok($"Email {email} is already in use");
        }

        [HttpGet("Name")]
        public async Task<IActionResult> IsUserNameInUse(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Ok(true);
            }
            return Ok($"UserName {userName} is already in use");
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

            // Check if the userName and password are not null
            if (loginDTO.UserName == null || loginDTO.Password == null)
            {
                return BadRequest("UserName and Password are required.");
            }


            // Check if the user exists
            var result = await _signInManager.PasswordSignInAsync(loginDTO.UserName, loginDTO.Password, isPersistent:loginDTO.RememberMe,lockoutOnFailure: false);
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

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var user = await _userManager.FindByNameAsync(changePasswordDTO.username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

        [HttpPut("ChangePhoneNumber")]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneDTO changePhoneDTO)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var user = await _userManager.FindByNameAsync(changePhoneDTO.username);
            if (user == null)
            {
                return NotFound("User not found.");
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


        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("User name is required.");
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound("User not found.");
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
