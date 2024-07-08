﻿using SushiSpace.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Entities
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public string? ImgUrl { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
