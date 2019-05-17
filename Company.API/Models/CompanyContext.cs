using Microsoft.EntityFrameworkCore;

namespace Company.API.Models
{
    public class CompanyContext : DbContext
    {
        public CompanyContext(DbContextOptions<CompanyContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
