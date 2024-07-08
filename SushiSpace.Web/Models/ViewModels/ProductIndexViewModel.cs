using SushiSpace.Web.Models.Entites;

namespace SushiSpace.Web.Models.ViewModels
{
    public class ProductIndexViewModel
    {
        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }

        public  List<CartProduct> cartProducts { get; set; }

        public CartViewModel Cart { get; set; }
    }
}
