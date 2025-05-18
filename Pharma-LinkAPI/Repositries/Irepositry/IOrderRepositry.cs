using Pharma_LinkAPI.Models;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface IOrderRepositry
    {
        Task<Order?> GetOrderById(int orderId);
        Task<IEnumerable<Order?>> GetAllOrdersForCompany(int companyId);
        Task<IEnumerable<Order?>> GetAllOrdersForPharmacy(int pharmacyId);
        Task ChangeStatusOrder(Order order, string newStatus);
        Task DeleteOrder(Order order);
        Task AddOrder(Order order);
    }
}
