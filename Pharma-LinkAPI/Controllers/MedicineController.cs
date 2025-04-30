using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.DTO;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.Identity;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAccountRepositry _accountRepositry;

        public MedicineController(AppDbContext context,IAccountRepositry accountRepositry)
        {
            _context = context;
            _accountRepositry = accountRepositry;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineDTO>>> GetMedicine()
        {
            var medicines = await _context.Medicines.ToListAsync();
            
            var ret = new List<MedicineDTO>();
            foreach (var medicine in medicines)
            {
                var medicineDTO = new MedicineDTO
                {
                    ID = medicine.ID,
                    Name = medicine.Name,
                    Description = medicine.Description,
                    Price = medicine.Price,
                    InStock = medicine.InStock,
                    Image_URL = medicine.Image_URL,
                    Company_Id = medicine.Company_Id

                };
                ret.Add(medicineDTO);
            }
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicineDTO>> GetMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);

            if (medicine == null)
            {
                return NotFound();
            }

            return new MedicineDTO
            {
                ID = medicine.ID,
                Name = medicine.Name,
                Description = medicine.Description,
                Price = medicine.Price,
                InStock = medicine.InStock,
                Image_URL = medicine.Image_URL,
                Company_Id = medicine.Company_Id
            };
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicine(int id, MedicineDTO medicineDTO)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            if(id != medicineDTO.ID)
            {
                return BadRequest();
            }
            medicine.Name = medicineDTO.Name;
            medicine.Description = medicineDTO.Description;
            medicine.Price = medicineDTO.Price;
            medicine.InStock = medicineDTO.InStock;
            medicine.Image_URL = medicineDTO.Image_URL;
            medicine.Company_Id = medicineDTO.Company_Id;
            _context.Entry(medicine).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicineDTOExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<MedicineDTO>> PostMedicine(MedicineDTO medicineDTO)
        {
            var med = new Medicine
            {
                Name = medicineDTO.Name,
                Description = medicineDTO.Description,
                Price = medicineDTO.Price,
                InStock = medicineDTO.InStock,
                Image_URL = medicineDTO.Image_URL,
            };
            var user  = await _accountRepositry.GetCurrentUser(User);
            //if (user == null || user.Role != SD.Role_Company)
            //{
            //    return NotFound();
            //}
            med.Company_Id = user.Id;
            _context.Medicines.Add(med);
            await _context.SaveChangesAsync();
            return Created();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicineDTOExists(int id)
        {
            return _context.Medicines.Any(e => e.ID == id);
        }
    }
}
