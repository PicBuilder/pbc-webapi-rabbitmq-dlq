using Microsoft.EntityFrameworkCore;

namespace Employee.EventBus.Data.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<EventBus.Entities.Employee> Employee { get; set; }
        int SaveChanges();
    }
}
