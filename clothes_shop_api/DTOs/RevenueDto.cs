using clothes_shop_api.DTOs.ProductDtos;

namespace clothes_shop_api.DTOs
{
    public class RevenueDto
    {
        public decimal MonthlyRevenue { get; set; } = 0;
        public int MonthlyOrders { get; set; } = 0;
        public dynamic BestSellingProducts { get; set; } = null;
        public dynamic UnshippedOrders { get; set; } = null;
        public dynamic CategorySales { get; set; } = null;
    }
}
