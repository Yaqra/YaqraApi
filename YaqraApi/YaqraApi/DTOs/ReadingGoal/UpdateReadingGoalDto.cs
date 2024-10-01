namespace YaqraApi.DTOs.ReadingGoal
{
    public class UpdateReadingGoalDto
    {
        public int Id { get; set; }
        public int NumberOfBooksToRead { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationInDays { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
    }
}
