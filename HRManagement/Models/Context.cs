using HR_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HRManagement.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SalaryRecord> SalaryRecords { get; set; }

    }
}
