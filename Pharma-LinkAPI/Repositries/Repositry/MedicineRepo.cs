using Humanizer;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
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
        public void Add(Medicine entity)
        {
            _db.Medicines.Add(entity);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var medicine = GetById(id);
            if (medicine != null)
            {
                medicine.IsDeleted = true;
                _db.SaveChanges();
            }
        }

        public IEnumerable<Medicine> GetAll()
        {
            return _db.Medicines.ToList();
        }

        public Medicine? GetById(int id)
        {
            return _db.Medicines.FirstOrDefault(m => m.ID == id);
        }

        public void Update(Medicine entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var medicine = GetById(entity.ID);
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
                    //_db.Medicines.Update(medicine);
                    _db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new Exception("Concurrency error occurred while updating the medicine.");
                }
            }

        }
        public IEnumerable<Medicine> Search(string word)
        {
            var medicines = _db.Medicines.AsNoTracking().Include(c => c.Company)
                 .Where(m => m.Name.Contains(word) || m.Company.Name.Contains(word)).ToList();
            return medicines;
        }

        public async Task<IDictionary<int, Medicine?>> GetMedicinesForCompany(int companyId)
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
    }
}
