namespace YaqraApi.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Picture { get; set; }
        public string? Bio { get; set; }
        //public double Rate { get; set; } => calculated
    }
}
