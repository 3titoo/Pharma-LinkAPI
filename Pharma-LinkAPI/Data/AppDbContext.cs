using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Services;
using System.Reflection.Emit;

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
        }

    }
}
