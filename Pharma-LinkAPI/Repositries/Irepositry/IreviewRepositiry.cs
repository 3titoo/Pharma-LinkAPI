using Pharma_LinkAPI.Models;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface IreviewRepositiry : Irepo<Review>
    {
        Task<Review?> GetReviewByphAndCo(int pharmacyId,int CompanyId);
        Task<IEnumerable<Review?>> GetReviewsByPharmacyId(int pharmacyId);

    }
}
