using Microsoft.EntityFrameworkCore;
using YaqraApi.Repositories.Context;

namespace YaqraApi.Helpers
{
    public static class DebugHelpers
    {
        public static void LogTrackedEntities(ApplicationContext _context)
        {
            var trackedEntities = _context.ChangeTracker.Entries();
            foreach (var entry in trackedEntities)
            {
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}, Id: {entry.Property("Id").CurrentValue}");
            }
        }
    }
}
