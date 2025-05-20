using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class OrderRepo : IOrderRepositry
    {
        private readonly AppDbContext _context;
        public OrderRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderOnlyById(int orderId)
        {
            var order = await _context.Orders.Include(o => o.Pharmacy)
                                        .FirstOrDefaultAsync(o => o.OrderID == orderId);
            return order;
        }

        public async Task<Order?> GetOrderAndOrderItemsById(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(ot => ot.Medicine)
                                        .Include(o => o.Pharmacy)
                                        .Include(o => o.Company)
                                        .FirstOrDefaultAsync(o => o.OrderID == orderId);
            return order;
        }
        public async Task<IEnumerable<Order?>> GetAllOrdersForCompany(int companyId)
        {
            var orders = await _context.Orders
                           .Include(o => o.Pharmacy)
                           .Where(o => o.CompanyID == companyId)
                           .ToListAsync();

            return orders;
        }
        public async Task<IEnumerable<Order?>> GetAllOrdersForPharmacy(int pharmacyId)
        {
            var orders = await _context.Orders
                           .Include(o => o.Company)
                           .Where(o => o.PharmacyID == pharmacyId)
                           .ToListAsync();

            return orders;
        }

        public async Task AddOrder(Order order)
        {
           await _context.Orders.AddAsync(order);

           await _context.SaveChangesAsync();
        }

        public async Task DeleteOrder(Order order)
        {

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
        }
        public async Task ChangeStatusOrder(Order order, string newStatus)
        {
            order.StatusOrder = newStatus;

            await _context.SaveChangesAsync();
        }
    }
}
