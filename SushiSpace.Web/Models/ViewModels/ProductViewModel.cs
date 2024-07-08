namespace SushiSpace.Web.Models.ViewModels
{
    public class ProductViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public string? ImgUrl { get; set; }
        
        public IFormFile? ImgFile { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
