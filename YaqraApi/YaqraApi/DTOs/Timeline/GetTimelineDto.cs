namespace YaqraApi.DTOs.Timeline
{
    public class GetTimelineDto
    {
        public bool Followings { get; set; } = false;
        public int page { get; set; } = 1;
    }
}
