using SushiSpace.Core.Helper.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Helper
{
    public class ProductQueryParameters : QueryParameters
    {
        public string? FilterByName { get; set; } // Имя продукта для поиска
        public float? MinPrice { get; set; } // Минимальная цена продукта
        public float? MaxPrice { get; set; } // Максимальная цена продукта
    }
}
