using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class SpecificationContext : DbContext
    {
        public DbSet<SpecificationModel> SpecificationModels { get; set; } = default;
        public DbSet<WarehouseRecordModel> WarehouseRecordModel { get; set; } = default;

        public DbSet<OrderModel> OrderModel { get; set; } = default!;

        public SpecificationContext (DbContextOptions<SpecificationContext> options)
            : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=D:\KIS\Spec.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpecificationModel>().HasOne(n => n.Parent).WithMany(n => n.Childrens).OnDelete(DeleteBehavior.Cascade).HasForeignKey(n => n.ParentId);
            modelBuilder.Entity<WarehouseRecordModel>().HasOne(wr => wr.SpecificationModel).WithMany(b => b.WarehouseRecords)
                .HasForeignKey(wr => wr.SpecificationModelId);
            modelBuilder.Entity<OrderModel>().HasOne(wr => wr.SpecificationModel).WithMany(b => b.Orders)
                .HasForeignKey(wr => wr.SpecificationModelId);

        }
    }
}
