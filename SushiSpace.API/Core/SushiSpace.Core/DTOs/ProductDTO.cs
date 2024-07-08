using FluentValidation;
using Microsoft.AspNetCore.Http;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.DTOs
{
    public record ProductDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public string? ImgUrl { get; set; }

        public int? CategoryId { get; set; }

        public IFormFile? ImgFile { get; set; }

    }
    public class ProductDTOValidator : AbstractValidator<ProductDTO> 
    {
        public ProductDTOValidator()
        {
            RuleFor(x=>x.Price).GreaterThanOrEqualTo(0).WithMessage("Price can not be less than 0");
            RuleFor(x => x.Name).MinimumLength(1).WithMessage("Name must contain at least 1 character");
        }
    }

}
