using SushiSpace.Web.Models.DTOs;

namespace SushiSpace.Web.Models.ViewModels
{
    public class ProductAndCategoryProductViewModel
    {
        public ProductViewModel Product { get; set; }
        public IEnumerable<ProductViewModel> RelatedProducts { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }
}
