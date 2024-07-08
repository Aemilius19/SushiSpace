namespace SushiSpace.Web.Models.Entites
{
    public class CartProduct
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }

        public int CategoryId { get; set; }
    }
}
