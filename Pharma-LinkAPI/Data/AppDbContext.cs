using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;

namespace Pharma_LinkAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region SeedRoles

            builder.Entity<AppRole>().HasData(
                new AppRole
                {
                    Id = 1,
                    Name = SD.Role_Admin,
                    NormalizedName = SD.Role_Admin.ToUpper()
                },
                new AppRole
                {
                    Id = 2,
                    Name = SD.Role_Pharmacy,
                    NormalizedName = SD.Role_Pharmacy.ToUpper()
                },
                new AppRole
                {
                    Id = 3,
                    Name = SD.Role_Company,
                    NormalizedName = SD.Role_Company.ToUpper()
                }
            );
            #endregion

            #region Review
            builder.Entity<Review>()
            .HasOne(r => r.pharmacy)
            .WithMany(p => p.ReviewsGiven)
            .HasForeignKey(r => r.PharmacyId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
            .HasOne(r => r.company)
            .WithMany(c => c.ReviewsReceived)
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region OrdersforPharmacy&Company
            builder.Entity<Order>()
                .HasOne(o => o.Pharmacy)
                .WithMany(u => u.Ordersrequested)
                .HasForeignKey(o => o.PharmacyID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
            .HasOne(o => o.Company)
            .WithMany(u => u.OrderReceived)
            .HasForeignKey(o => o.CompanyID)
            .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region DeletedOrder&Cart
            builder.Entity<Order>()
                    .HasMany(o => o.OrderItems)
                    .WithOne(ot => ot.Order)
                    .HasForeignKey(ot => ot.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ct => ct.Cart)
                .HasForeignKey(ct => ct.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion


            #region softDelete
            builder.Entity<Medicine>().HasQueryFilter(m => m.IsDeleted == false);
            #endregion

        }
    }
}
