using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.DTO;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Transactions;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext Context;
        private readonly UserManager<AppUser> MyUsers;
        public OrderController(AppDbContext _context, UserManager<AppUser> _users)
        {
            Context = _context;
            MyUsers = _users;
        }

        [HttpGet("IndexCompanyOrder/{CompanyId:int}")]
        public async Task<ActionResult<IEnumerable<CompanyInvoiceDTO>>> IndexCompanyOrder(int CompanyId)
        {
            var orders = Context.Orders.Include(o => o.Pharmacy)
                                       .Include( o => o.OrderItems )
                                       .ThenInclude( ot => ot.Medicine )
                                       .Where( o => o.CompanyID == CompanyId );

            var CompanyInvoices = new List<CompanyInvoiceDTO>();
            foreach( var item in orders )
            {
                var TempCompanyInvoice = new CompanyInvoiceDTO
                {
                    PharmacyName = item.Pharmacy.Name,
                    DRName = item.Pharmacy.DrName,
                    PharmacyPhone = item.Pharmacy.PhoneNumber,
                    Street = item.Pharmacy.Street,
                    State = item.Pharmacy.State,
                    City = item.Pharmacy.City,
                    OrderDate = (DateOnly)item.OrderDate,
                    StatusOrder = item.StatusOrder
                };
                CompanyInvoices.Add(TempCompanyInvoice);
            }
            return Ok(CompanyInvoices);

        }

        [HttpGet("{OrderId:int}")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoice(int OrderId)
        {
            var order = await Context.Orders.Include(o => o.OrderItems).ThenInclude( ot => ot.Medicine)
                                        .Include(o => o.Pharmacy)
                                        .Include(o => o.Company)
                                        .FirstOrDefaultAsync(o => o.OrderID == OrderId);

            var company = await MyUsers.FindByIdAsync(order.CompanyID.ToString());

            var Invoice = new InvoiceDTO
            {
                DRName = order.Pharmacy.DrName,
                Phone = order.Pharmacy.PhoneNumber,
                PharmacyLicense = order.Pharmacy.LiscnceNumber,
                PharmacyName = order.Pharmacy.Name,
                CompanyName = company.Name,
                CompanyLicense = company.LiscnceNumber,
                Street = order.Pharmacy.Street,
                State = order.Pharmacy.State,
                City = order.Pharmacy.City,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                StatusOrder = SD.StatusOrder_pending,
                TotalPriceOrder = order.TotalPrice,
                Medicines = new List<InvoiceMedicineDTO>()
            };
            foreach( var item in order.OrderItems )
            {
                var InvoiceMedicineDTO = new InvoiceMedicineDTO
                {
                    Name = item.Medicine.Name,
                    Image_URL = item.Medicine.Image_URL,
                    UnitPrice = item.Medicine.Price,
                    Count = item.Count,
                    TotalPrice = item.TotalPrice
                };
                Invoice.Medicines.Add(InvoiceMedicineDTO);
            }
            return Ok(Invoice);
        }

        [HttpPost("add/{CartId:int}")]
        public async Task<ActionResult<InvoiceDTO>> PlaceOrder( int CartId , int companyId )
        {

            using var transaction = await Context.Database.BeginTransactionAsync(); // Begin transaction

            try
            {
                var CurrentCart = await Context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(c => c.Medicine)
                    .FirstOrDefaultAsync(c => c.CartId == CartId);

                var Medicines = await Context.Medicines.Where(m => m.Company_Id == companyId).ToListAsync();

                if (CurrentCart == null)
                    return NotFound("Cart not found.");

                // First check all the items on the card.
                foreach (var item in CurrentCart.CartItems)
                {
                    Medicine CurMedicine = Medicines.FirstOrDefault(m => m.ID == item.MedicineId);

                    var availableStock = CurMedicine.InStock;

                    if (availableStock == null || availableStock < item.Count)
                    {
                        return BadRequest($"Quantity not available for the medicine Name = {item.Medicine.Name}.");
                    }
                }

                // All quantities are available - we start creating the order
                Order newOrder = new Order
                {
                    OrderItems = new List<OrderItem>(),
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    StatusOrder = SD.StatusOrder_pending,
                    PharmacyID = CurrentCart.PharmacyId,
                    CompanyID = companyId,
                    TotalPrice = 0
                };

                foreach (var item in CurrentCart.CartItems)
                {

                    // Quantity discount
                    Medicine CurMedicine = Medicines.FirstOrDefault(m => m.ID == item.MedicineId);

                    CurMedicine.InStock -= item.Count;

                    OrderItem newOrderItem = new OrderItem
                    {
                        MedicineID = item.MedicineId,
                        Count = item.Count,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.Count * item.UnitPrice
                    };

                    newOrder.TotalPrice += newOrderItem.TotalPrice;
                    newOrder.OrderItems.Add(newOrderItem);
                }

                Context.Orders.Add(newOrder);
                Context.Carts.Remove(CurrentCart);

                // Save changes to the database
                await Context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                return CreatedAtAction("GetInvoice", new { OrderId = newOrder.OrderID }, newOrder);
            }
            catch (Exception ex)
            {

                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync(); 

                return StatusCode(500, $"An error occurred while executing the request.: {ex.Message}");
            }
        }


        [HttpPut("done/{OrderId:int}")]
        public async Task<ActionResult<InvoiceDTO>> DoneOrder(int OrderId)
        {
            var order = await Context.Orders.Include(o => o.OrderItems).ThenInclude(ot => ot.Medicine).ThenInclude(m => m.Company)
                                        .Include(o => o.Pharmacy)
                                        .Include(o => o.Company)
                                        .FirstOrDefaultAsync(o => o.OrderID == OrderId);
            order.StatusOrder = SD.StatusOrder_shipped;

            var company = await MyUsers.FindByIdAsync(order.CompanyID.ToString());

            var Invoice = new InvoiceDTO
            {
                DRName = order.Pharmacy.DrName,
                Phone = order.Pharmacy.PhoneNumber,
                PharmacyLicense = order.Pharmacy.LiscnceNumber,
                PharmacyName = order.Pharmacy.Name,
                CompanyName = company.Name,
                CompanyLicense = company.LiscnceNumber,
                Street = order.Pharmacy.Street,
                State = order.Pharmacy.State,
                City = order.Pharmacy.City,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                StatusOrder = SD.StatusOrder_shipped,
                TotalPriceOrder = order.TotalPrice,
                Medicines = new List<InvoiceMedicineDTO>()
            };
            foreach (var item in order.OrderItems)
            {
                var InvoiceMedicineDTO = new InvoiceMedicineDTO
                {
                    Name = item.Medicine.Name,
                    Image_URL = item.Medicine.Image_URL,
                    UnitPrice = item.Medicine.Price,
                    Count = item.Count,
                    TotalPrice = item.TotalPrice
                };
                Invoice.Medicines.Add(InvoiceMedicineDTO);
            }

            await Context.SaveChangesAsync();

            return Ok(Invoice);
        }

        [HttpDelete(("{OrderId:int}"))]
        public async Task<ActionResult<InvoiceDTO>> DeleteOrder(int OrderId)
        {
            var order = await Context.Orders
                                        .FirstOrDefaultAsync(o => o.OrderID == OrderId);
            Context.Orders.Remove(order);
            await Context.SaveChangesAsync();
            return Content("The request has been cleared successfully");
        }

    }
}
