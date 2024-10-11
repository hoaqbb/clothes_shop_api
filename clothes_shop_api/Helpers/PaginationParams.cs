namespace clothes_shop_api.Helpers
{
    public class PaginationParams
    {
        //private const int MaxPageSize = 8;
        public int PageNumber { get; set; } = 1;
        //private int _pageSize = 8;
        public int PageSize { get; } = 8;

        //public int PageSize
        //{
        //    get => _pageSize;
        //    set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        //}
    }
}
