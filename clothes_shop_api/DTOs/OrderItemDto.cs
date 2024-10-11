namespace clothes_shop_api.DTOs
{
    public class OrderItemDto
    {
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        
        //public int Total { get; set; }
    }
}
