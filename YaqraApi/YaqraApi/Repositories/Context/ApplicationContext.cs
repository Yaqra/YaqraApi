﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using YaqraApi.Models;
using YaqraApi.Models.Enums;

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


                u.HasMany(x => x.FavouriteAuthors)
                    .WithMany(a => a.Users)
                    .UsingEntity("UserFavouriteAuthors",
                    l => l.HasOne(typeof(Author))
                        .WithMany().HasForeignKey("AuthorId"),
                    r => r.HasOne(typeof(ApplicationUser))
                        .WithMany().HasForeignKey("UserId"),
                    j => j.HasKey("UserId", "AuthorId"));

                u.HasMany(x => x.ReadingGoals)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);


                builder.Entity<ApplicationUser>()
                .OwnsMany(u => u.RefreshTokens);

            });

            builder.Entity<Book>(u =>
            {
                u.HasMany(x => x.Genres)
                .WithMany(g => g.Books)
                .UsingEntity("BooksGenres",
                l => l.HasOne(typeof(Genre))
                    .WithMany().HasForeignKey("GenreId"),
                r => r.HasOne(typeof(Book))
                    .WithMany().HasForeignKey("BookId"),
                j => j.HasKey("BookId", "GenreId"));

                u.HasMany(x => x.Authors)
                .WithMany(g => g.Books)
                .UsingEntity("BooksAuthors",
                l => l.HasOne(typeof(Author))
                    .WithMany().HasForeignKey("AuthorId"),
                r => r.HasOne(typeof(Book))
                    .WithMany().HasForeignKey("BookId"),
                j => j.HasKey("BookId", "AuthorId"));

            });

            builder.Entity<UserBookWithStatus>().HasKey(ub => new { ub.UserId, ub.BookId });

            builder.Entity<Genre>().HasIndex(x => x.Name).IsUnique();

            base.OnModelCreating(builder);
        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<ReadingGoal> ReadingGoals { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
