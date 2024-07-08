using SushiSpace.Web.Models.Entites;

namespace SushiSpace.Web.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartProduct> CartProducts { get; set; }
        public IEnumerable<ProductViewModel> RelatedProducts { get; set; }
    }
}
