namespace SushiSpace.Web.Models.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public string? ImgUrl { get; set; }

        public int? CategoryId { get; set; }

        public IFormFile? ImgFile { get; set; }
    }
}
