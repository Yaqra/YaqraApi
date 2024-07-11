﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using YaqraApi.Models;

namespace YaqraApi.Repositories.Context
{
    public class ApplicationContext:IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(u =>
            {
                u.HasMany(u => u.Followers)
                .WithMany(u => u.Followings)
                .UsingEntity("UsersFollowers",
                l => l.HasOne(typeof(ApplicationUser))
                    .WithMany().HasForeignKey("UserId"),
                r => r.HasOne(typeof(ApplicationUser))
                    .WithMany().HasForeignKey("FollowerId"),
                j => j.HasKey("UserId", "FollowerId"));

            });

            base.OnModelCreating(builder);
        }
    }
}
