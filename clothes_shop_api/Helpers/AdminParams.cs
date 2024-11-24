namespace clothes_shop_api.Helpers
{
    public class AdminParams : PaginationParams
    {
        public string SortBy { get; set; } = "created_descending";
        public string Category { get; set; } = "all";
    }

    public class AdminProductParams : PaginationParams
    {
        public string SortBy { get; set; } = "created_descending";
        public string Category { get; set; } = "all";
        public string Status { get; set; } = "all";
    }

    public class AdminOrderParams : PaginationParams
    {
        public string SortBy { get; set; } = "created_descending";
        public string Status { get; set; } = "all";
        public string PaymentMethod { get; set; } = "all";
    }
}
