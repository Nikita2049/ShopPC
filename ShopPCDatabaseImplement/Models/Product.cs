using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShopPCDatabaseImplement.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public decimal Price { get; set; }
        public virtual List<ProductComponent> ProductComponents { get; set; }
        public virtual List<Order> Orders { get; set; }
    }
}