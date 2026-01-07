using App.SuperAdmin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Master.DBContext
{
    public class MasterDbContext(DbContextOptions<MasterDbContext> options) : DbContext(options)
    {
        public DbSet<IndustryConfig> IndustryConfig { get; set; }
        public virtual DbSet<MasterDropDown> MasterDropDowns { get; set; }
        public virtual DbSet<DropDownDetails> DropDownDetails { get; set; }
    }
}
