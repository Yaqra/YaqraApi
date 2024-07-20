namespace YaqraApi.Models
{
    public class ReadingGoal
    {
        public int Id { get; set; }
        public int NumberOfBooksToRead { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationInDays { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public DateTime EndDate => StartDate.AddDays(DurationInDays);
        public string UserId { get; set; }
        public ApplicationUser User{ get; set; }
    }
}
