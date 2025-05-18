using Pharma_LinkAPI.Models;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface IreviewRepositiry : Irepo<Review>
    {
        Review? GetReviewByphAndCo(int pharmacyId, int CompanyId);
        IEnumerable<Review?> GetReviewsByPharmacyId(int pharmacyId);

    }
}
