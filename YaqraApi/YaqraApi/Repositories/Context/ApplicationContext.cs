using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YaqraApi.Models;

namespace YaqraApi.Repositories.Context
{
    public class ApplicationContext:IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
