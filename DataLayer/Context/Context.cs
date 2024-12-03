using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Context
{
    public class PoolsContext : DbContext
    {
        public PoolsContext(DbContextOptions<PoolsContext> options) : base(options) { }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BrandCategory> BrandCategories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>().Property(e => e.Image).HasDefaultValue("default.jpg");
            modelBuilder.Entity<Category>().Property(e => e.Image).HasDefaultValue("default.jpg");
            modelBuilder.Entity<Product>().Property(e => e.Image).HasDefaultValue("default.jpg");

            modelBuilder.Entity<BrandCategory>()
           .HasKey(bc => new { bc.BrandID, bc.CategoryID });

            modelBuilder.Entity<BrandCategory>()
                .HasOne(bc => bc.Brand)
                .WithMany(b => b.BrandCategories)
                .HasForeignKey(bc => bc.BrandID);

            modelBuilder.Entity<BrandCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.BrandCategories)
                .HasForeignKey(bc => bc.CategoryID);

            base.OnModelCreating(modelBuilder);
        }

    }

}