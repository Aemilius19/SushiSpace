using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
