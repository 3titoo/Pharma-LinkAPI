using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.DTO;
using Pharma_LinkAPI.Identity;

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
        private readonly IWebHostEnvironment _env;

        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _env = env;

            if (!_roleManager.RoleExistsAsync("Pharmacy").Result)
            {
                var role = new AppRole { Name = $"{SD.Role_Pharmacy}" };
                _roleManager.CreateAsync(role).Wait();

                var role2 = new AppRole { Name = $"{SD.Role_Admin}" };
                _roleManager.CreateAsync(role2).Wait();

                var role3 = new AppRole { Name = $"{SD.Role_Company}" };
                _roleManager.CreateAsync(role3).Wait();
            }
        }

        [HttpPost("PhRegister")]
        public async Task<IActionResult> Register(PharmacyRegisterDTO pharmacyRegisterDTO,IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

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
                pdfPath = filePath
            };
            var result = await _userManager.CreateAsync(user, pharmacyRegisterDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Role_Pharmacy);
                return Ok(new { Message = "User registered successfully." });
            }
            string error = string.Join(" | ", result.Errors.Select(x => x.Description));
            return BadRequest(error);
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Ok(true);
            }
            return Ok($"Email {email} is already in use");
        }

        [HttpGet]
        public async Task<IActionResult> IsUserNameInUse(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Ok(true);
            }
            return Ok($"UserName {userName} is already in use");
        }

    }
}
