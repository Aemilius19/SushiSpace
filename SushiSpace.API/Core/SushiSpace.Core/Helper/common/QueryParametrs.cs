using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Helper.common
{
    public abstract class QueryParameters
    {
        public string? SortBy { get; set; } // Поле для сортировки
        public bool SortDescending { get; set; } // Флаг для сортировки по убыванию
    }
}
