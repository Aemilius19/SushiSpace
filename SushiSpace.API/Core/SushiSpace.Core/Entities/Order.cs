using Microsoft.EntityFrameworkCore.Infrastructure;
using SushiSpace.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Entities
{
    public class Order:BaseEntity
    {
        
        public User User { get; set; } 

        public List<OrderProduct>? Products { get; set; }

        public string Status { get; set; }

        public string Adress { get; set; }

        public string PaymentStatus { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
