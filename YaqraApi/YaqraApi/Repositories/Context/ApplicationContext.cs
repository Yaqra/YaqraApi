using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection.Emit;
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

                u.OwnsMany(u => u.Connections);

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

                u.HasMany(x => x.Playlists)
                .WithMany(g => g.Books)
                .UsingEntity("PlaylistBooks",
                l => l.HasOne(typeof(Playlist))
                    .WithMany().HasForeignKey("PlaylistId"),
                r => r.HasOne(typeof(Book))
                    .WithMany().HasForeignKey("BookId"),
                j => j.HasKey("BookId", "PlaylistId"));

                u.HasMany(x => x.DiscussionArticleNews)
                .WithMany(g => g.Books)
                .UsingEntity("DiscussionArticleNewsBooks",
                l => l.HasOne(typeof(DiscussionArticleNews))
                    .WithMany().HasForeignKey("DiscussionArticleNewsId"),
                r => r.HasOne(typeof(Book))
                    .WithMany().HasForeignKey("BookId"),
                j => j.HasKey("BookId", "DiscussionArticleNewsId"));

            });

            builder.Entity<Comment>(u =>
            {
                u.HasMany(u => u.Replies)
                .WithOne(u => u.ParentComment)
                .HasForeignKey(u => u.ParentCommentId);
            });
            builder.Entity<UserBookWithStatus>().HasKey(ub => new { ub.UserId, ub.BookId });

            builder.Entity<Genre>().HasIndex(x => x.Name).IsUnique();

            builder.Entity<Post>().UseTptMappingStrategy();
            builder.Entity<RecommendationStatistics>().HasKey(r => new { r.UserId, r.GenreId });

            builder.Entity<PostLikes>(p =>
            {
                p.HasOne(u => u.User)
                .WithMany(u => u.PostLikes)
                .HasForeignKey(u => u.UserId);

                p.HasOne(u => u.Post)
                .WithMany(u => u.PostLikes)
                .HasForeignKey(u => u.PostId);

                p.HasKey(k => new { k.UserId, k.PostId });
            });

            builder.Entity<CommentLikes>(c =>
            {
                c.HasOne(u => u.User)
                .WithMany(u => u.CommentLikes)
                .HasForeignKey(u => u.UserId);

                c.HasOne(u => u.Comment)
                .WithMany(u => u.CommentLikes)
                .HasForeignKey(u => u.CommentId);

                c.HasKey(k => new { k.UserId, k.CommentId });
            });

            builder.Entity<Notification>()
                .HasOne(n => n.Post)
                .WithMany(p => p.Notifications)
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommentLikes>()
                .HasOne(c => c.Comment)
                .WithMany(c=>c.CommentLikes)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<ReadingGoal> ReadingGoals { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<DiscussionArticleNews> DiscussionArticleNews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RecommendationStatistics> RecommendationStatistics { get; set; }
        public DbSet<TrendingBook> TrendingBooks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PostLikes> PostLikes { get; set; }
        public DbSet<CommentLikes> CommentLikes { get; set; }

    }
}
