using Microsoft.AspNetCore.Identity;
using Pharma_LinkAPI.Identity;

namespace Pharma_LinkAPI.Services
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Define roles to seed
            var roles = new[] { SD.Role_Admin, SD.Role_Pharmacy, SD.Role_Company };

            // Seed roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var appRole = new AppRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    };
                    await roleManager.CreateAsync(appRole);
                }
            }

            // Define the admin user details
            var adminEmail = "admin@gmail.com";
            var adminPassword = "Admin@123";

            // Check if the admin user already exists
            var userExist = await userManager.FindByEmailAsync(adminEmail);
            if (userExist == null)
            {
                var adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Name = "Admin",
                    PhoneNumber = "01554715848",
                    EmailConfirmed = true,
                    Role = SD.Role_Admin
                };

                // Create the admin user
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Assign the Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors));
                }
            }
        }
    }
}
