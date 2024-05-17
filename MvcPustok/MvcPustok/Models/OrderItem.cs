using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcPustok.Models
{
	public class OrderItem:BaseEntity
	{
        public int Count { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }      
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal SalePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercent { get; set; }
        public Order? Order { get; set; }
        public Book? Book { get; set; }

        public decimal CalculateTotalPrice()
        {
            decimal discountedPrice = SalePrice - (SalePrice * (DiscountPercent / 100));
            return discountedPrice * Count;
        }
    }
}

