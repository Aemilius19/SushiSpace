using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.DTOs
{
    public record CategoryDTO
    {
        public string Name { get; set; }
        
    }

    public class CategoryDTOValidator : AbstractValidator<CategoryDTO> 
    {
        public CategoryDTOValidator()
        {
            RuleFor(x=>x.Name).NotEmpty().WithMessage("name can not be null");
        }
    }

}
