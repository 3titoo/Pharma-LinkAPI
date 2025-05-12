using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;
using System.ComponentModel.Design;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class OrderRepo : IOrderRepositry
    {
        private readonly AppDbContext _context;
        public OrderRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderById(int orderId)
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

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);

            _context.SaveChanges();
        }

        public void DeleteOrder(Order order)
        {
            
            _context.Orders.Remove(order);
    
            _context.SaveChanges();
        }
        public void ChangeStatusOrder(Order order, string newStatus)
        {
            order.StatusOrder = newStatus;

            _context.SaveChanges();
        }
    }
}
