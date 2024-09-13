namespace YaqraApi.Helpers
{
    public static class Pagination
    {
        public static int AuthorNamesAndIds = 36; 
        public static int BookTitlesAndIds = 36; 
        public static int UserFollowersNames = 36; 
        public static int UserFollowingsNames = 36;
        public static int Genres = 36;
        public static int Authors = 18; 
        public static int Books = 18; 
        public static int ReadingGoals = 18; 
        public static int Posts = 18; 
        public static int Comments = 27; 
        public static int RecommendedBooks = 9; 
        public static int TrendingBooks = 9; 
        public static int Timeline = 27; 
        public static int Notifications = 36; 

        public static int CalculatePagesCount(int totalElements, int pageSize)
        {
            if (pageSize == 0)
                return 1;

            return (int)Math.Ceiling((float)totalElements / (float)pageSize);
        }

    }
}
