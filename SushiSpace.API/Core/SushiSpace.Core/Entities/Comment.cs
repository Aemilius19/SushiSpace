using SushiSpace.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Entities
{
    public class Comment:BaseEntity
    {
        
        public string Text { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        public User User { get; set; } // Assuming you have a User entity
        
    }
}
