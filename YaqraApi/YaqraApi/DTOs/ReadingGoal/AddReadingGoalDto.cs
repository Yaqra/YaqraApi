namespace YaqraApi.DTOs.ReadingGoal
{
    public class AddReadingGoalDto
    {
        public int NumberOfBooksToRead { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationInDays { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
    }
}
