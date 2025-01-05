using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektFinAssist.Models;

namespace ProjektFinAssist.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ProjektFinAssist.Models.OperationsModel> OperationsModel { get; set; } = default!;
    }
}
