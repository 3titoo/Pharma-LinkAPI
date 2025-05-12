using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using System.Threading.Tasks;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface IOrderRepositry
    {
        Task<Order?> GetOrderById(int orderId);
        Task<IEnumerable<Order?>> GetAllOrdersForCompany(int companyId);
        Task<IEnumerable<Order?>> GetAllOrdersForPharmacy(int pharmacyId);
        void ChangeStatusOrder(Order order, string newStatus);
        void DeleteOrder(Order order);
        void AddOrder(Order order);
    }
}
