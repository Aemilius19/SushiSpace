using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.DTOs
{
    public record RegisterDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; } 

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("email field is required").Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            RuleFor(x=>x.Password).NotEmpty().WithMessage("password field is required").Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$").WithMessage("Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character");
            RuleFor(x=>x.ConfirmPassword).NotEmpty().WithMessage("password field is required").Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$").WithMessage("Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character").Equal(x=>x.Password).WithMessage("password field and confirm password must be same");
        }
    }

}
