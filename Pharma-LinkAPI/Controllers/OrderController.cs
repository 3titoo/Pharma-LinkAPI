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
using static NuGet.Packaging.PackagingConstants;
using Microsoft.AspNetCore.Authorization;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext Context;
        private readonly UserManager<AppUser> MyUsers;
        private readonly IAccountRepositry _account;
        public OrderController(AppDbContext _context, UserManager<AppUser> _users,IAccountRepositry account)
        {
            Context = _context;
            MyUsers = _users;
            _account = account;
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpGet("IndexCompanyOrder/{CompanyId:int}")]
        public async Task<ActionResult<IEnumerable<CompanyInvoiceDTO>>> IndexCompanyOrder(int CompanyId)
        {
            var currentUser = await _account.GetCurrentUser(User);
            if (currentUser.Id != CompanyId)
            {
                return Problem("You are not authorized to view this order.");
            }

            var orders = Context.Orders.Include(o => o.Pharmacy)                                      
                                       .Where(o => o.CompanyID == CompanyId);

            if(orders == null)
            {
                return NotFound("Orders for Company not found.");
            }

            var CompanyInvoices = new List<CompanyInvoiceDTO>();
            foreach (var item in orders)
            {
                var TempCompanyInvoice = new CompanyInvoiceDTO
                {
                    OrderID = item.OrderID,
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

        [Authorize(Roles = SD.Role_Pharmacy)]
        [HttpGet("IndexPharmacyOrder/{PharmacyId:int}")]
        public async Task<ActionResult<IEnumerable<PharmacyInvoiceDTO>>> IndexPharmacyOrder(int PharmacyId)
        {
            var currentUser = await _account.GetCurrentUser(User);
            if (currentUser.Id != PharmacyId)
            {
                return Problem("You are not authorized to view this order.");
            }

            var orders = Context.Orders.Include(o => o.Company)
                                       .Where(o => o.PharmacyID == PharmacyId);

            if (orders == null)
            {
                return NotFound("Orders for Pharmacy not found.");
            }

            var PharmacyInvoices = new List<PharmacyInvoiceDTO>();
            foreach (var item in orders)
            {
                var TempPharmacyInvoice = new PharmacyInvoiceDTO
                {
                    OrderID = item.OrderID,
                    CompanyName = item.Company.Name,
                    Street = item.Company.Street,
                    State = item.Company.State,
                    City = item.Company.City,
                    OrderDate = (DateOnly)item.OrderDate,
                    StatusOrder = item.StatusOrder
                };
                PharmacyInvoices.Add(TempPharmacyInvoice);
            }
            return Ok(PharmacyInvoices);

        }

        [Authorize]
        [HttpGet("Invoice/{OrderId:int}")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoice(int OrderId)
        {
            var order = await Context.Orders.Include(o => o.OrderItems).ThenInclude(ot => ot.Medicine)
                                        .Include(o => o.Pharmacy)
                                        .Include(o => o.Company)
                                        .FirstOrDefaultAsync(o => o.OrderID == OrderId);

            var currentUser = await _account.GetCurrentUser(User);

            if(currentUser.Id != order.CompanyID && currentUser.Id != order.PharmacyID)
            {
                return Problem("You are not authorized to view this order.");
            }

            if (order == null)
            {
                return NotFound("Orders not found.");
            }

            if(order.OrderItems == null)
            {
                return NotFound("Order items not found.");
            }

            if(order.Company == null)
            {
                return NotFound("Company not found.");
            }

            if (order.Pharmacy == null)
            {
                return NotFound("Pharmacy not found.");
            }

            var Invoice = new InvoiceDTO
            {
                OrderID = OrderId,
                DRName = order.Pharmacy.DrName,
                Phone = order.Pharmacy.PhoneNumber,
                PharmacyLicense = order.Pharmacy.LiscnceNumber,
                PharmacyName = order.Pharmacy.Name,
                CompanyName = order.Company.Name,
                CompanyLicense = order.Company.LiscnceNumber,
                Street = order.Pharmacy.Street,
                State = order.Pharmacy.State,
                City = order.Pharmacy.City,
                OrderDate = order.OrderDate,
                StatusOrder = SD.StatusOrder_pending,
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
            return Ok(Invoice);
        }

        [Authorize(Roles = SD.Role_Pharmacy)]
        [HttpPost("PlaceOrder/{CartId:int}/{companyId:int}")]
        public async Task<ActionResult<InvoiceDTO>> PlaceOrder(int CartId, int companyId)
        {

            using var transaction = await Context.Database.BeginTransactionAsync(); // Begin transaction

            try
            {
                var CurrentCart = await Context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(c => c.Medicine)
                    .FirstOrDefaultAsync(c => c.CartId == CartId);
                if (CurrentCart == null)
                {
                    return NotFound("Cart not found.");
                }

                if(CurrentCart.CartItems == null)
                {
                    return NotFound("Cart items not found.");
                }

                var MedicinesForCompany = await Context.Medicines.Where(m => m.Company_Id == companyId)
                                                                 .ToDictionaryAsync(m => m.ID);

                if(MedicinesForCompany == null)
                {
                    return NotFound("Medicines for Company not found.");
                }


                // First check all the items on the card.
                foreach (var item in CurrentCart.CartItems)
                {
                    Medicine CurMedicine = MedicinesForCompany[item.MedicineId.Value];

                    if(CurMedicine == null)
                    {
                        return NotFound("Medicine not found.");
                    }

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
                    Medicine CurMedicine = MedicinesForCompany[item.MedicineId.Value];

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

                var CurCartItems = CurrentCart.CartItems.ToList();

                // Deleted the CartItems
                Context.CartItems.RemoveRange(CurCartItems);


                CurrentCart.TotalPrice = 0;

                var company = await MyUsers.FindByIdAsync(companyId.ToString());
                var pharmacy = await MyUsers.FindByIdAsync(CurrentCart.PharmacyId.ToString());

                if (company == null)
                {
                    return NotFound("Company not found.");
                }

                if (pharmacy == null)
                {
                    return NotFound("Pharmacy not found.");
                }

                var Invoice = new InvoiceDTO
                {
                    OrderID = newOrder.OrderID,
                    DRName = pharmacy.DrName,
                    Phone = pharmacy.PhoneNumber,
                    PharmacyLicense = pharmacy.LiscnceNumber,
                    PharmacyName = pharmacy.Name,
                    CompanyName = company.Name,
                    CompanyLicense = company.LiscnceNumber,
                    Street = pharmacy.Street,
                    State = pharmacy.State,
                    City = pharmacy.City,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    StatusOrder = SD.StatusOrder_pending,
                    TotalPriceOrder = newOrder.TotalPrice,
                    Medicines = new List<InvoiceMedicineDTO>()
                };
                foreach (var item in newOrder.OrderItems)
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

                // Save changes to the database
                await Context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                return CreatedAtAction("GetInvoice", new { OrderId = newOrder.OrderID }, Invoice);
            }
            catch (Exception ex)
            {

                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();

                return StatusCode(500, $"An error occurred while executing the request.: {ex.Message}");
            }
        }

        [Authorize(Roles = SD.Role_Company)]
        [HttpPut("done/{OrderId:int}")]
        public async Task<ActionResult<InvoiceDTO>> DoneOrder(int OrderId)
        {
            var order = await Context.Orders.Include(o => o.OrderItems).ThenInclude(ot => ot.Medicine).ThenInclude(m => m.Company)
                                        .Include(o => o.Pharmacy)
                                        .Include(o => o.Company)
                                        .FirstOrDefaultAsync(o => o.OrderID == OrderId);

            var currentUser = await _account.GetCurrentUser(User);
            if( currentUser.Id != order.CompanyID)
            {
                return Problem("You are not authorized to view this order.");
            }

            if (order == null)
            {
                return NotFound("Orders not found.");
            }

            if (order.OrderItems == null)
            {
                return NotFound("Order items not found.");
            }

            if (order.Company == null)
            {
                return NotFound("Company not found.");
            }

            if (order.Pharmacy == null)
            {
                return NotFound("Pharmacy not found.");
            }

            if (order.StatusOrder != SD.StatusOrder_pending)
            {
                return BadRequest($"Order is {order.StatusOrder}");
            }

            order.StatusOrder = SD.StatusOrder_shipped;


            var Invoice = new InvoiceDTO
            {
                OrderID = OrderId,
                DRName = order.Pharmacy.DrName,
                Phone = order.Pharmacy.PhoneNumber,
                PharmacyLicense = order.Pharmacy.LiscnceNumber,
                PharmacyName = order.Pharmacy.Name,
                CompanyName = order.Company.Name,
                CompanyLicense = order.Company.LiscnceNumber,
                Street = order.Pharmacy.Street,
                State = order.Pharmacy.State,
                City = order.Pharmacy.City,
                OrderDate = order.OrderDate,
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

        [Authorize(Roles = SD.Role_Company)]
        [HttpPut("deliver/{OrderId:int}")]
        public async Task<ActionResult<InvoiceDTO>> DeliverOrder(int OrderId)
        {
            var order = await Context.Orders.Include(o => o.OrderItems).ThenInclude(ot => ot.Medicine).ThenInclude(m => m.Company)
                                        .Include(o => o.Pharmacy)
                                        .Include(o => o.Company)
                                        .FirstOrDefaultAsync(o => o.OrderID == OrderId);

            var currentUser = await _account.GetCurrentUser(User);
            if (currentUser.Id != order.CompanyID)
            {
                return Problem("You are not authorized to view this order.");
            }

            if (order == null)
            {
                return NotFound("Orders not found.");
            }

            if (order.OrderItems == null)
            {
                return NotFound("Order items not found.");
            }

            if (order.Company == null)
            {
                return NotFound("Company not found.");
            }

            if (order.Pharmacy == null)
            {
                return NotFound("Pharmacy not found.");
            }

            if (order.StatusOrder != SD.StatusOrder_shipped)
            {
                return BadRequest($"Order is {order.StatusOrder}");
            }

            order.StatusOrder = SD.StatusOrder_delivered;


            var Invoice = new InvoiceDTO
            {
                OrderID = OrderId,
                DRName = order.Pharmacy.DrName,
                Phone = order.Pharmacy.PhoneNumber,
                PharmacyLicense = order.Pharmacy.LiscnceNumber,
                PharmacyName = order.Pharmacy.Name,
                CompanyName = order.Company.Name,
                CompanyLicense = order.Company.LiscnceNumber,
                Street = order.Pharmacy.Street,
                State = order.Pharmacy.State,
                City = order.Pharmacy.City,
                OrderDate = order.OrderDate,
                StatusOrder = SD.StatusOrder_delivered,
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


        [Authorize(Roles = SD.Role_Company)]
        [HttpDelete("Cancel/{OrderId:int}")]
        public async Task<ActionResult> CancelOrder(int OrderId)
        {
            using var transaction = await Context.Database.BeginTransactionAsync(); // Begin transaction

            try
            {
                var order = await Context.Orders.Include(o => o.OrderItems)
                              .FirstOrDefaultAsync(o => o.OrderID == OrderId);

                var currentUser = await _account.GetCurrentUser(User);
                if (currentUser.Id != order.CompanyID)
                {
                    return Problem("You are not authorized to view this order.");
                }

                if (order == null)
                {
                    return NotFound("Orders not found.");
                }

                if (order.OrderItems == null)
                {
                    return NotFound("Order items not found.");
                }

                if (order.StatusOrder != SD.StatusOrder_pending)
                {
                    return BadRequest($"Order is {order.StatusOrder}");
                }

                if (order.CompanyID == null)
                {
                    return NotFound("Company not found.");
                }

                if (order.PharmacyID == null)
                {
                    return NotFound("Pharmacy not found.");
                }

                var companyId = order.CompanyID;
                var Medicines = await Context.Medicines.Where(m => m.Company_Id == companyId).ToListAsync();

                if(Medicines == null)
                {
                    return NotFound("Medicines for Company not found.");
                }

                foreach (var item in order.OrderItems)
                {

                    // Return the quantity
                    Medicine CurMedicine = Medicines.FirstOrDefault(m => m.ID == item.MedicineID);
                    CurMedicine.InStock += item.Count;
                }

                Context.Orders.Remove(order);

                // Save changes to the database
                await Context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                return Content("The order has been successfully cancelled.");

            }
            catch (Exception ex)
            {

                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();

                return StatusCode(500, $"An error occurred while executing the request.: {ex.Message}");
            }
            
        }

    }
}