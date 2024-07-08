using FluentValidation;
using Newtonsoft.Json;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.DTOs
{
    public record OrderDTO
    {
        public string UserId { get; set; }

        public string Status { get; set; } 

        public string Adress { get; set; }

        public string PaymentStatus { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        

        public List<OrderProduct>? Products { get; set; }
    }

    public class OrderValidator : AbstractValidator<OrderDTO>
    {
        public OrderValidator()
        {
            RuleFor(x => x.Adress).NotEmpty().WithMessage("adress must not be null");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("phone must not be null");
        }
    }
}
