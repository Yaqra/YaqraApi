using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;

namespace YaqraApi.Helpers
{
    public static class UserHelpers
    {
        public static string GetUserId(ClaimsPrincipal User)
        {
            return User?.FindFirst(c => c.Type == "uid")?.Value;
        }
        public static string GetErrors(IdentityResult result)
        {
            var errors = new StringBuilder();
            foreach (var error in result.Errors)
                errors.Append($"{error.Description}, ");
            return errors.ToString();
        }
    }
}
