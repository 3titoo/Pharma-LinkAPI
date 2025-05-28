using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IrequestRepositry _requestRepositry;
        private readonly IUnitOfWork _unitOfWork;


        public RequestsController(IrequestRepositry irequestRepositry,IUnitOfWork unitOfWork)
        {
            _requestRepositry = irequestRepositry;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register(PharmacyRequestDTO pharmacyRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

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
                UserName = pharmacyRegisterDTO.UserName,
                Password = pharmacyRegisterDTO.Password
            };
            string? password = pharmacyRegisterDTO.Password;
            if (password == null || password[0] == ' ')
            {
                return BadRequest("Password is required.");
            }
            await _requestRepositry.Add(requset);
            _ = Task.Run(async () =>
            {
                await _unitOfWork._emailService.SendEmailAsync(
                    requset.Email,
                    "Pharmacy Email Confirmation",
                    $"To confirm your email, please click <a href=\"https://mozakeer.github.io/PharmaLink/confirm-email?email={requset.Email}\">here</a>"
                );
            });
            return Ok("request is added");
        }





        // GET: api/Requests
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            var requests = await _requestRepositry.GetAll();
            return Ok(requests);
        }
        [Authorize(Roles = SD.Role_Admin)]
        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request?>> GetRequest(int id)
        {
            var request = await _requestRepositry.GetById(id);

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }



        // DELETE: api/Requests/5
        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _requestRepositry.GetById(id);
            if (request == null)
            {
                return NotFound();
            }

            await _requestRepositry.Delete(id);

            return NoContent();
        }

        [HttpPatch("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }
            var request = await _requestRepositry.GetUserByEmail(email);
            if (request == null)
            {
                return NotFound("Request not found.");
            }
            request.IsEmailConfirmed = true;
            await _requestRepositry.Update(request);
            return Ok("Email confirmed successfully.");
        }
    }
}
