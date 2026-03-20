using Microsoft.EntityFrameworkCore;

namespace mywebapi.Models
{
    public class EmpContext : DbContext  // <- inherit DbContext
    {
        public EmpContext(DbContextOptions<EmpContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}