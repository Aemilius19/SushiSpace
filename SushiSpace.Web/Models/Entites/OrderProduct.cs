namespace SushiSpace.Web.Models.Entites
{
    public class OrderProduct
    {
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }
}
