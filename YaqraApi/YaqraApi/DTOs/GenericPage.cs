namespace YaqraApi.DTOs
{
    public class GenericPage<T>
    {
        public T Data { get; set; }
        public int PagesCount { get; set; }
        public int PageNumber { get; set; }
    }
}
