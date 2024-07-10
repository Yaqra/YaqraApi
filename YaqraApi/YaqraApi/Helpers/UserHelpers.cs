using System.Security.Claims;

namespace YaqraApi.Helpers
{
    public static class UserHelpers
    {
        public static string GetUserId(ClaimsPrincipal User)
        {
            return User?.FindFirst(c => c.Type == "uid")?.Value;
        }
    }
}
