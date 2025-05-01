using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAccountRepositry _accountRepositry;
        public MedicineController(ImedicineRepositiry medicineRepositiry, IAccountRepositry accountRepositry)
        {
            _medicineRepositiry = medicineRepositiry;
            _accountRepositry = accountRepositry;
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
                    var user = await _accountRepositry.GetUserById(medicine.Company_Id.Value);
                    if (user != null)
                    {
                        item.CompanyName = user.Name;
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
            var user = await _accountRepositry.GetUserById(medicine.Company_Id.Value);
            if (user != null)
            {
                res.CompanyName = user.Name;
            }
            return Ok(medicine);
        }

        //[Authorize(Roles = SD.Role_Company)]
        [HttpPost]
        public async Task<ActionResult<string>> addMedicine(MedicineDTO medicine) {
            #region Image
            var img = medicine.img;
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
            var medicineModel = new Medicine
            {
                Name = medicine.Name,
                Description = medicine.Description,
                Price = medicine.Price,
                InStock = medicine.InStock,
                //Image_URL = imgPath,
            };
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            medicineModel.Company_Id = int.Parse(userId);
            _medicineRepositiry.Add(medicineModel);
            return Ok("Medicine added successfully");
        }

        [HttpPut("{id}")]
        public ActionResult<string> UpdateMedicine(int id, MedicineDTO medicine)
        {
            var existingMedicine = _medicineRepositiry.GetById(id);
            if (existingMedicine == null)
            {
                return NotFound();
            }
            existingMedicine.Name = medicine.Name;
            existingMedicine.Description = medicine.Description;
            existingMedicine.Price = medicine.Price;
            existingMedicine.InStock = medicine.InStock;
            if (medicine.img != null)
            {
                var img = medicine.img;
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
                existingMedicine.Image_URL = imgPath;
            }
            _medicineRepositiry.Update(existingMedicine);
            return Ok("Medicine updated successfully");
        }

        [HttpDelete("{id}")]
        public ActionResult<string> DeleteMedicine(int id)
        {
            var existingMedicine = _medicineRepositiry.GetById(id);
            if (existingMedicine == null)
            {
                return NotFound();
            }
            _medicineRepositiry.Delete(existingMedicine.ID);
            return Ok("Medicine deleted successfully");
        }
    }
}
