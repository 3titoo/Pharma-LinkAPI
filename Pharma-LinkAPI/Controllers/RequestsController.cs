using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.Services.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IrequestRepositry _requestRepositry;
        private readonly UserManager<AppUser> _userManager;


        public RequestsController(IrequestRepositry irequestRepositry, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _requestRepositry = irequestRepositry;
        }

        [HttpPost("Register")]
        public ActionResult<string> Register(PharmacyRequestDTO pharmacyRegisterDTO)
        {
            var existEmail = _userManager.FindByEmailAsync(pharmacyRegisterDTO.Email).Result;
            if (existEmail != null)
            {
                return BadRequest("Email is already in use");
            }
            var existUserName = _userManager.FindByNameAsync(pharmacyRegisterDTO.UserName).Result;
            if (existUserName != null)
            {
                return BadRequest("User name is already in use");
            }
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            var img = pharmacyRegisterDTO.img;

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

            var requset = new Request()
            {
                Pharmacy_License = pharmacyRegisterDTO.LicenseNumber,
                DR_Name = pharmacyRegisterDTO.DrName,
                Pharmacy_Name = pharmacyRegisterDTO.Name,
                Phone = pharmacyRegisterDTO.PhoneNumber,
                Email = pharmacyRegisterDTO.Email,
                Street = pharmacyRegisterDTO.Street,
                State = pharmacyRegisterDTO.State,
                City = pharmacyRegisterDTO.City,
                License_File = pharmacyRegisterDTO.pdfURL,
                ImageUrl = imgPath,
                UserName = pharmacyRegisterDTO.UserName,
                Password = pharmacyRegisterDTO.Password
            };
            string? password = pharmacyRegisterDTO.Password;
            if (password == null || password[0] == ' ')
            {
                return BadRequest("Password is required.");
            }
            _requestRepositry.Add(requset);
            return Ok("request is added");
        }





        // GET: api/Requests
        [HttpGet]
        public  ActionResult<IEnumerable<Request>> GetRequests()
        {
            var requests =  _requestRepositry.GetAll();
            return Ok(requests);
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public ActionResult<Request?> GetRequest(int id)
        {
            var request =  _requestRepositry.GetById(id);

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }



        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRequest(int id)
        {
            var request =  _requestRepositry.GetById(id);
            if (request == null)
            {
                return NotFound();
            }

            _requestRepositry.Delete(id);

            return NoContent();
        }
    }
}
