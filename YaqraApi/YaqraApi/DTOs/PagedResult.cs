namespace YaqraApi.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } // Current page data
        public int PageNumber { get; set; } // Current page number
        public int PageSize { get; set; } // Number of items per page
        public int TotalPages { get; set; } // Total number of pages
    }
}
