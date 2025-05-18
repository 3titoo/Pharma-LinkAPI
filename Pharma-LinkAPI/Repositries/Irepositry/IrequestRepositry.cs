using Pharma_LinkAPI.Models;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface IrequestRepositry : Irepo<Request>
    {
        Task<Request?> GetUserByEmail(string email);
        Task<Request?> GetUserByusername(string username);

    }
}
