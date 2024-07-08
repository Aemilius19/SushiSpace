using SushiSpace.Web.Models.Entites;

namespace SushiSpace.Web.Models.DTOs
{
    public class OrderDTO
    {
        public string UserId { get; set; }

        public string Status { get; set; } = "Pending";

        public string Adress { get; set; }

        public string PaymentStatus { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public List<OrderProduct>? Products { get; set; }
    }
}
