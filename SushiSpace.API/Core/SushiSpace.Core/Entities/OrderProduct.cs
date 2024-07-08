using SushiSpace.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Entities
{
    public class OrderProduct:BaseEntity
    {
        public int? OrderId { get; set; }
        public int ProductId { get; set; }
        public Product? Products { get; set; }     
        public int Quantity { get; set; }
        public double Price { get; set; } // Цена на момент добавления в заказ
    }
}
