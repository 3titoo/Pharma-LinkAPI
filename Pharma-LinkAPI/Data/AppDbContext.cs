using Microsoft.EntityFrameworkCore;

namespace Pharma_LinkAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        
    }
}
