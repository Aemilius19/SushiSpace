using SushiSpace.Web.Models.Entites;

namespace SushiSpace.Web.Models.ViewModels
{
    public class ConfirmPaymentViewModel
    {
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<OrderProduct> Products { get; set; }
        public string Total { get; set; }
    }
}
