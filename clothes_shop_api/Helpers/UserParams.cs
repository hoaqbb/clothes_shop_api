namespace clothes_shop_api.Helpers
{
    public class UserParams : PaginationParams
    {
        public string SortBy { get; set; } = "created_descending";
    }

    public class UserProductParams : PaginationParams
    {
        public string category { get; set; } = "all";
        public string SortBy { get; set; } = "created_descending";
    }
}
