using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.DTOs
{
    public class OrderProductDTO
    {
        public int? OrderId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }
}
