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
                    .WithMany().HasForeignKey("FollowerId"),
                r => r.HasOne(typeof(ApplicationUser))
                    .WithMany().HasForeignKey("FollowedId"),
                j => j.HasKey("FollowerId", "FollowedId"));

                u.Property(x => x.UserName).IsRequired();

            });

            builder.Entity<ApplicationUser>(u =>
            {
                u.HasMany(x => x.FavouriteGenres)
                .WithMany(g => g.Users)
                .UsingEntity("UserFavouriteGenres",
                l => l.HasOne(typeof(Genre))
                    .WithMany().HasForeignKey("GenreId"),
                r => r.HasOne(typeof(ApplicationUser))
                    .WithMany().HasForeignKey("UserId"),
                j => j.HasKey("UserId", "GenreId"));

                builder.Entity<ApplicationUser>()
                .OwnsMany(u => u.RefreshTokens);

            });

            builder.Entity<Genre>().HasIndex(x => x.Name).IsUnique();

            base.OnModelCreating(builder);
        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
