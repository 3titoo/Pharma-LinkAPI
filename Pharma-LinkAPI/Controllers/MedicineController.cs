using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.DTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pharma_LinkAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly ImedicineRepositiry _medicineRepositiry;
        private readonly IUnitOfWork _unitOfWork;
        public MedicineController(ImedicineRepositiry medicineRepositiry,IUnitOfWork unitOfWork)
        {
            _medicineRepositiry = medicineRepositiry;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineViewModel>>> GetAllMedicines()
        {
            var medicines = _medicineRepositiry.GetAll();

            var ret = new List<MedicineViewModel>();

            foreach (var medicine in medicines)
            {
                var item = new MedicineViewModel
                {
                    Id = medicine.ID,
                    MedicineName = medicine.Name,
                    Description = medicine.Description,
                    Price = medicine.Price,
                    InStock = medicine.InStock,
                    ImageUrl = medicine.Image_URL,
                };
                if (medicine.Company_Id != null)
                {
                    var user = await _unitOfWork._accountRepositry.GetUserById(medicine.Company_Id.Value);
                    if (user != null)
                    {
                        item.CompanyName = user.Name;
                        item.CompanyUserName = user.UserName;
                    }
                }
                ret.Add(item);
            }
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicineViewModel>> GetMedicineById(int id)
        {
            var medicine = _medicineRepositiry.GetById(id);
            if (medicine == null)
            {
                return NotFound();
            }
            var res = new MedicineViewModel
            {
                Id = medicine.ID,
                MedicineName = medicine.Name,
                Description = medicine.Description,
                Price = medicine.Price,
                InStock = medicine.InStock,
                ImageUrl = medicine.Image_URL,
            };
            var user = await _unitOfWork._accountRepositry.GetUserById(medicine.Company_Id.Value);
            if (user != null)
            {
                res.CompanyName = user.Name;
                res.CompanyUserName = user.UserName;
            }
            return Ok(res);
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpPost]
        public async Task<ActionResult<string>> addMedicine([FromForm] MedicineDTO medicine)
        {
            #region Image
            var img = medicine.Image;
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var medicineModel = new Medicine
            {
                Name = medicine.Name,
                Description = medicine.Description,
                Price = medicine.Price,
                InStock = medicine.InStock,
                Company_Id = userId,
                Image_URL = imgPath,
            };

            _medicineRepositiry.Add(medicineModel);
            return Ok("Medicine added successfully");
        }
        [Authorize(Roles = SD.Role_Company)]
        [HttpPut("{id}")]
        public ActionResult<string> UpdateMedicine(int id, MedicinePutDTO medicine)
        {
            var user = _unitOfWork._accountRepositry.GetCurrentUser(User);
            var existingMedicine = _medicineRepositiry.GetById(id);
            if (existingMedicine == null)
            {
                return NotFound();
            }
            if(existingMedicine.Company_Id != user.Id)
            {
                return BadRequest("You are not allowed to update another company Products");
            }
            existingMedicine.Name = medicine.Name;
            existingMedicine.Description = medicine.Description;
            existingMedicine.Price = medicine.Price;
            existingMedicine.InStock = medicine.InStock;
            _medicineRepositiry.Update(existingMedicine);
            return Ok("Medicine updated successfully");
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteMedicine(int id)
        {
            var user = _unitOfWork._accountRepositry.GetCurrentUser(User);

            var existingMedicine = _medicineRepositiry.GetById(id);
            if (existingMedicine == null)
            {
                return NotFound();
            }
            if (existingMedicine.Company_Id != user.Id)
            {
                return BadRequest("You are not allowed to remove another company Products");
            }
            var items = await _unitOfWork._cartRepositry.GetCartItemsByMedicineId(id);
            await _unitOfWork._cartRepositry.RemoveCartItems(items);
            _medicineRepositiry.Delete(existingMedicine.ID);
            return Ok("Medicine deleted successfully");
        }


        [HttpGet("search")]
        public ActionResult<IEnumerable<SearchDTO>> search(string? word)
        {
            if(string.IsNullOrEmpty(word))
            {
                return NoContent();
            }
            var medicines = _medicineRepositiry.Search(word);
            if (medicines == null)
            {
                return NoContent();
            }
            var ret = new List<SearchDTO>();
            foreach (var medicine in medicines)
            {
                var item = new SearchDTO
                {
                    Name = medicine.Name,
                    CompanyName = medicine.Company.Name,
                    Price = medicine.Price,
                    InStock = medicine.InStock,
                    img = medicine.Image_URL,
                };
                ret.Add(item);
            }
            return Ok(ret);
        }


        [Authorize(Roles = SD.Role_Company)]
        [HttpPatch("updateStock")]
        public ActionResult<string> UpdateStock(editMedicineQuantityDTO dto)
        {
            var user = _unitOfWork._accountRepositry.GetCurrentUser(User);
            var medicne = _medicineRepositiry.GetById(dto.medicineId);
            if (medicne == null)
            {
                return NotFound("Medicine not found.");
            }

            if (medicne.Company_Id != user.Id)
            {
                return BadRequest("You are not allowed to update another company Products");
            }

            medicne.InStock = dto.quantity;
            _medicineRepositiry.Update(medicne);
            return Ok("Medicine stock updated successfully.");
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpPatch("uploadPhoto/{id}")]
        public async Task<IActionResult> uploadPhoto(int id,IFormFile? img)
        {
            var medicne = _medicineRepositiry.GetById(id);
            if (medicne == null)
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
            var imgPath = Path.Combine(uploadsFolder, img.FileName);
            using (var stream = new FileStream(imgPath, FileMode.Create))
            {
                img.CopyTo(stream);
            }
            #endregion

            medicne.Image_URL = imgPath;
            _medicineRepositiry.Update(medicne);
            return NoContent();
        }

    }

}
