using ShopPCDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ShopPCDatabaseImplement
{
    public class ShopPCDatabase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-NVT3S6Q\SQLEXPRESS;Initial Catalog=ShopPCDatabase;Integrated Security=True;MultipleActiveResultSets=True;");
            }
            base.OnConfiguring(optionsBuilder);
        }
        public virtual DbSet<Component> Components { set; get; }
        public virtual DbSet<Product> Products { set; get; }
        public virtual DbSet<ProductComponent> ProductComponents { set; get; }
        public virtual DbSet<Order> Orders { set; get; }
    }
}