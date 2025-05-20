using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.DTO.MdeicineDTO;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class MedicineRepo : ImedicineRepositiry
    {
        private readonly AppDbContext _db;
        public MedicineRepo(AppDbContext db)
        {
            _db = db;
        }
        public async Task Add(Medicine? entity)
        {
            await _db.Medicines.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var medicine = await GetById(id);
            if (medicine != null)
            {
                medicine.IsDeleted = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Medicine?>> GetAll()
        {
            return await _db.Medicines.AsNoTracking().ToListAsync();
        }

        public async Task<Medicine?> GetById(int id)
        {
            return await _db.Medicines.FirstOrDefaultAsync(m => m.ID == id);
        }

        public async Task Update(Medicine? entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var medicine = await GetById(entity.ID);
            if (medicine != null)
            {
                try
                {
                    medicine.Name = entity.Name;
                    medicine.Description = entity.Description;
                    medicine.Price = entity.Price;
                    medicine.InStock = entity.InStock;
                    medicine.Image_URL = entity.Image_URL;
                    medicine.Company_Id = entity.Company_Id;
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new Exception("Concurrency error occurred while updating the medicine.");
                }
            }

        }
        public async Task<IEnumerable<Medicine>> Search(string word)
        {
            var medicines = await _db.Medicines.AsNoTracking().Include(c => c.Company)
                 .Where(m => m.Name.Contains(word) || m.Company.Name.Contains(word)).ToListAsync();
            return medicines;
        }

        public async Task<IDictionary<int, MedicineViewDTO>> GetMedicinesForCompany(int companyId)
        {
            var medicines = await _db.Medicines.Where(m => m.Company_Id == companyId).Select(c => new MedicineViewDTO
            {
                Id = c.ID,
                MedicineName = c.Name,
                Description = c.Description,
                Price = c.Price,
                InStock = c.InStock,
                ImageUrl = c.Image_URL,
                CompanyName = c.Company.Name,
                CompanyUserName = c.Company.UserName
            })
            .ToDictionaryAsync(m => m.Id);

            return medicines;
        }

        public async Task<IDictionary<int, Medicine>> GetMedicinesForCompanyTracking(int companyId)
        {
            var medicines = await _db.Medicines.Where(m => m.Company_Id == companyId).Include(m => m.Company)
                                                                 .ToDictionaryAsync(m => m.ID);

            return medicines;
        }

        public async Task<Medicine?> GetMedicineCompany(int Id)
        {
            var med = await _db.Medicines
                  .Include(m => m.Company).Where(d => Id == d.ID).FirstOrDefaultAsync();
            return med;
        }

        public async Task<(string Name,string UserName)?> GetCompanyDetails(int id)
        {
            var company = _db.Users.Where(c => c.Id == id)
                          .Select(c => new {c.Name,c.UserName}).FirstOrDefault();
            if (company == null)
                return null;

            return (company.Name, company.UserName);
        }
    }
}
