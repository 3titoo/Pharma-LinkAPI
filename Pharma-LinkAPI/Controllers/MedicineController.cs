using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.DTO.MdeicineDTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;
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
        public MedicineController(ImedicineRepositiry medicineRepositiry, IUnitOfWork unitOfWork)
        {
            _medicineRepositiry = medicineRepositiry;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineViewDTO>>> GetAllMedicines()
        {
            var medicines = await _medicineRepositiry.GetAll();

            var ret = new List<MedicineViewDTO>();

            foreach (var medicine in medicines)
            {
                var item = new MedicineViewDTO
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
                    var user = await _unitOfWork._medicineRepositiry.GetCompanyDetails(medicine.Company_Id.Value);
                    if (user != null)
                    {
                        item.CompanyName = user.Value.Name;
                        item.CompanyUserName = user.Value.UserName;
                    }
                }
                ret.Add(item);
            }
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicineViewDTO>> GetMedicineById(int id)
        {
            var medicine = await _medicineRepositiry.GetById(id);
            if (medicine == null)
            {
                return NotFound();
            }
            var res = new MedicineViewDTO
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
            var user = await _unitOfWork._accountRepositry.GetCurrentUser(User);

            var medicineModel = new Medicine
            {
                Name = medicine.Name,
                Description = medicine.Description,
                Price = medicine.Price,
                InStock = medicine.InStock,
                Company_Id = user.Id,
                Image_URL = imgPath,
            };

            await _medicineRepositiry.Add(medicineModel);
            return Ok("Medicine added successfully");
        }
        [Authorize(Roles = SD.Role_Company)]
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateMedicine(int id, MedicinePutDTO medicine)
        {
            var user = await _unitOfWork._accountRepositry.GetCurrentUser(User);
            var existingMedicine = await _medicineRepositiry.GetById(id);
            if (existingMedicine == null)
            {
                return NotFound();
            }
            if (existingMedicine.Company_Id != user.Id)
            {
                return BadRequest("You are not allowed to update another company Products");
            }
            existingMedicine.Name = medicine.Name;
            existingMedicine.Description = medicine.Description;
            existingMedicine.Price = medicine.Price;
            existingMedicine.InStock = medicine.InStock;
            await _medicineRepositiry.Update(existingMedicine);
            return Ok("Medicine updated successfully");
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteMedicine(int id)
        {
            var user = await _unitOfWork._accountRepositry.GetCurrentUser(User);

            var existingMedicine = await _medicineRepositiry.GetById(id);
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
            await _medicineRepositiry.Delete(existingMedicine.ID);
            return Ok("Medicine deleted successfully");
        }


        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SearchDTO>>> search(string? word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return NoContent();
            }
            var medicines = await _medicineRepositiry.Search(word);
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
        public async Task<ActionResult<string>> UpdateStock(editMedicineQuantityDTO dto)
        {
            var user = await _unitOfWork._accountRepositry.GetCurrentUser(User);
            var medicne = await _medicineRepositiry.GetById(dto.medicineId);
            if (medicne == null)
            {
                return NotFound("Medicine not found.");
            }

            if (medicne.Company_Id != user.Id)
            {
                return BadRequest("You are not allowed to update another company Products");
            }

            medicne.InStock = dto.quantity;
            await _medicineRepositiry.Update(medicne);
            return Ok("Medicine stock updated successfully.");
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpPatch("uploadPhoto/{id}")]
        public async Task<IActionResult> uploadPhoto(int id, IFormFile? img)
        {
            var medicne = await _medicineRepositiry.GetById(id);
            var user = await _unitOfWork._accountRepositry.GetCurrentUser(User);
            if (medicne == null)
            {
                return NotFound("User not found");
            }
            if (img == null || img.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            if (user.Id != medicne.Company_Id)
            {
                return BadRequest("You are not allowed to update another company Products");
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

            medicne.Image_URL = imgPath;
            await _medicineRepositiry.Update(medicne);
            return NoContent();
        }


        [HttpGet("CompanyMedicnines")]
        [Authorize(Roles = SD.Role_Company)]
        public async Task<ActionResult<IEnumerable<MedicineViewDTO>>> GetCompanyMedicines()
        {
            var user = await _unitOfWork._accountRepositry.GetCurrentUser(User);
            var medicinesDictionary =
                await _medicineRepositiry.GetMedicinesForCompany(user.Id);

            var ret = new List<MedicineViewDTO>();

            foreach (var medicine in medicinesDictionary.Values)
            {
                var item = new MedicineViewDTO
                {
                    Id = medicine.Id,
                    MedicineName = medicine.MedicineName,
                    Description = medicine.Description,
                    Price = medicine.Price,
                    InStock = medicine.InStock,
                    ImageUrl = medicine.ImageUrl,
                    CompanyName = user.Name,
                    CompanyUserName = user.UserName
                };
                ret.Add(item);
            }
            return ret;
        }

    }

}
