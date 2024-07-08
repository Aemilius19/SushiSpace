using SushiSpace.Core.Helper.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Helper
{
    public class CategoryQueryParameters : QueryParameters
    {
        public string? FilterByName { get; set; } // Фильтр по имени категории
    }
}
