using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.DTOs
{
    public record CommentDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int ProductId { get; set; }
        public string? UserName { get; set; }
    }

    public class CommentDTOValidator : AbstractValidator<CommentDTO> {

        public CommentDTOValidator()
        {
            RuleFor(x => x.Text).MinimumLength(1).WithMessage("Can not send nullable comment");
        }
    }

}
