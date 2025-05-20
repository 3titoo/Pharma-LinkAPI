using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class ReviewRepo : IreviewRepositiry
    {
        private readonly AppDbContext _db;
        public ReviewRepo(AppDbContext db)
        {
            _db = db;
        }
        public async Task Add(Review? entity)
        {
            await _db.Reviews.AddAsync(entity);
            await _db.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(i => i.Id == id);
            if (review != null)
            {
                _db.Reviews.Remove(review);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Review?>> GetAll()
        {
            var reviews = await _db.Reviews.ToListAsync();
            return reviews;
        }
        public async Task<Review> GetById(int id)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(i => i.Id == id);
            if (review == null)
            {
                throw new Exception("Review not found");
            }
            return review;
        }

        public async Task<Review?> GetReviewByphAndCo(int pharmacyId, int CompanyId)
        {
            var review = await _db.Reviews.AsNoTracking()
                .FirstOrDefaultAsync(r => r.PharmacyId == pharmacyId && r.CompanyId == CompanyId);
            return review;
        }

        public async Task<IEnumerable<Review?>> GetReviewsByPharmacyId(int pharmacyId)
        {
            var reviews = await _db.Reviews
                .Where(r => r.PharmacyId == pharmacyId)
                .ToListAsync();
            return reviews;
        }

        public async Task Update(Review? entity)
        {
            throw new NotImplementedException();
        }
    }
}
