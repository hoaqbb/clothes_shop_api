namespace clothes_shop_api.Helpers
{
    public class UserParams : PaginationParams
    {
        public string SortBy { get; set; } = "created_descending";
    }
}
