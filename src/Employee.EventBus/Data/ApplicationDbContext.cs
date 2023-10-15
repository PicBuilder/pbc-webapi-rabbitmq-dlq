using Employee.EventBus.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Employee.EventBus.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EventBus.Entities.Employee> Employee { get; set; } = default!;

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
