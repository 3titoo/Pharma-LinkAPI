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
        public void Add(Review entity)
        {
            _db.Reviews.Add(entity);
            _db.SaveChanges();
        }
        public void Delete(int id)
        {
            var review = _db.Reviews.FirstOrDefault(i => i.Id == id);
            if (review != null)
            {
                _db.Reviews.Remove(review);
                _db.SaveChanges();
            }
        }
        public IEnumerable<Review> GetAll()
        {
            var reviews = _db.Reviews.ToList();
            return reviews;
        }
        public Review GetById(int id)
        {
            var review = _db.Reviews.FirstOrDefault(i => i.Id == id);
            if (review == null)
            {
                throw new Exception("Review not found");
            }
            return review;
        }

        public Review? GetReviewByphAndCo(int pharmacyId, int CompanyId)
        {
            var review = _db.Reviews
                .FirstOrDefault(r => r.PharmacyId == pharmacyId && r.CompanyId == CompanyId);
            return review;
        }

        public IEnumerable<Review?> GetReviewsByPharmacyId(int pharmacyId)
        {
            var reviews = _db.Reviews
                .Where(r => r.PharmacyId == pharmacyId)
                .ToList();
            return reviews;
        }

        public void Update(Review entity)
        {
            throw new NotImplementedException();
        }
    }
}
