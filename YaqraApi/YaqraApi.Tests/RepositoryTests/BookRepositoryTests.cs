using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaqraApi.DTOs.Book;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
using YaqraApi.Repositories;
using YaqraApi.Repositories.Context;
using YaqraApi.Services;

namespace YaqraApi.Tests.RepositoryTests
{
    [TestFixture]
    public class BookRepositoryTests
    {
        private ApplicationContext _context;
        private BookRepository _repository;
        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName: "testDb")
                .Options;
            _context = new ApplicationContext(options);
            _repository = new BookRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [Test]
        public async Task GetRecent_ShouldReturnBooksAddedDuringLastWeek()
        {
            SeedDatabase();

            var result = await _repository.GetRecent(1);

            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.IsTrue(result.All(b => b.AddedDate >= DateTime.UtcNow.AddDays(-7)));
        }
        [Test]
        public async Task GetReviews_ShouldReturnBookReviewsOrderedAscendinglyBasedOnLikeCount()
        {
            SeedDatabase();
            var result = await _repository.GetReviews(1, 1, SortType.Ascending, ReviewsSortField.LikeCount);

            Assert.That(result, Is.Ordered.By("LikeCount"));
        }
        [Test]
        public async Task GetReviews_ShouldReturnBookReviewsOrderedDescendinglyBasedOnLikeCount()
        {
            SeedDatabase();
            var result = await _repository.GetReviews(1, 1, SortType.Descending, ReviewsSortField.LikeCount);

            Assert.That(result, Is.Ordered.By("LikeCount").Descending);
        }
        [Test]
        public async Task GetReviews_ShouldReturnBookReviewsOrderedAscendinglyBasedOnCreatedDate()
        {
            SeedDatabase();
            var result = await _repository.GetReviews(1, 1, SortType.Ascending, ReviewsSortField.CreatedDate);

            Assert.That(result, Is.Ordered.By("CreatedDate"));
        }
        [Test]
        public async Task GetReviews_ShouldReturnBookReviewsOrderedDescendinglyBasedOnCreatedDate()
        {
            SeedDatabase();
            var result = await _repository.GetReviews(1, 1, SortType.Descending, ReviewsSortField.CreatedDate);

            Assert.That(result, Is.Ordered.By("CreatedDate").Descending);
        }
        [Test]
        public async Task GetReviews_ShouldReturnBookReviewsOrderedAscendinglyBasedOnRate()
        {
            SeedDatabase();
            var result = await _repository.GetReviews(1, 1, SortType.Ascending, ReviewsSortField.Rate);

            Assert.That(result, Is.Ordered.By("Rate"));
        }
        [Test]
        public async Task GetReviews_ShouldReturnBookReviewsOrderedDescendinglyBasedOnRate()
        {
            SeedDatabase();
            var result = await _repository.GetReviews(1, 1, SortType.Descending, ReviewsSortField.Rate);

            Assert.That(result, Is.Ordered.By("Rate").Descending);
        }
        [Test]
        public async Task GetTrendingBooks_ShouldReturnMostTrendedBooks()
        {
            SeedDatabase();
            var result = await _repository.GetTrendingBooks();

            CollectionAssert.AreEqual(new List<int> { 2, 1 }, result.Select(b => b.Id));
        }
        [Test]
        public async Task FindBooks_ShouldReturnAllBooks_WhenAuthorIdsGenreIdsAndMinimumRateAreNull()
        {
            SeedDatabase();
            var dto = new BookFinderDto();
            
            var result = await _repository.FindBooks(dto, new BookProxyService(_repository));

            Assert.That(result.Count(), Is.EqualTo(4));
        }
        [Test]
        public async Task FindBooks_ShouldReturnBooksWithGenreFantazy_WhenGenreIdsExistAndAuthorIdsMinimumRateAreNull()
        {
            SeedDatabase();
            var dto = new BookFinderDto {GenreIds = new HashSet<int> {1} };

            var result = await _repository.FindBooks(dto, new BookProxyService(_repository));

            Assert.That(result.Count(), Is.EqualTo(2));
        }
        [Test]
        public async Task FindBooks_ShouldReturnBooksWithGenreFantazyAndAuthorTwo_WhenGenreIdsAndAuthorIdsExistAndMinimumRateIsNull()
        {
            SeedDatabase();
            var dto = new BookFinderDto { GenreIds = new HashSet<int> { 1 }, AuthorIds = new HashSet<int> { 2 } };

            var result = await _repository.FindBooks(dto, new BookProxyService(_repository));

            Assert.That(result.Count(), Is.EqualTo(0));
        }
        private void SeedDatabase()
        {
            // Create genres
            var genres = new List<Genre>
    {
        new Genre { Id = 1, Name = "Fantasy" },
        new Genre { Id = 2, Name = "Science Fiction" }
    };

            // Create authors
            var authors = new List<Author>
    {
        new Author { Id = 1, Name = "Author One" },
        new Author { Id = 2, Name = "Author Two" }
    };

            // Create users
            var users = new List<ApplicationUser>
    {
        new ApplicationUser { Id = "user1", UserName = "user1" },
        new ApplicationUser { Id = "user2", UserName = "user2" }
    };

            // Create books
            var books = new List<Book>
    {
        new Book
        {
            Id = 1,
            Title = "Book One",
            Description = "Description for Book One",
            NumberOfPages = 350,
            AddedDate = DateTime.UtcNow.AddDays(-1),
            Genres = new List<Genre> { genres[0] },
            Authors = new List<Author> { authors[0] }
        },
        new Book
        {
            Id = 2,
            Title = "Book Two",
            Description = "Description for Book Two",
            NumberOfPages = 400,
            AddedDate = DateTime.UtcNow.AddDays(-2),
            Genres = new List<Genre> { genres[1] },
            Authors = new List<Author> { authors[1] }
        },
        new Book
        {
            Id = 3,
            Title = "Book Three",
            Description = "Description for Book Three",
            NumberOfPages = 400,
            AddedDate = DateTime.UtcNow.AddDays(-2),
            Genres = new List<Genre> { genres[0] },
            Authors = new List<Author> { authors[0] }
        },
        new Book
        {
            Id = 4,
            Title = "Book Four",
            Description = "Description for Book Four",
            NumberOfPages = 400,
            AddedDate = DateTime.UtcNow.AddDays(-8),
            Genres = new List<Genre> { genres[1] },
            Authors = new List<Author> { authors[1] }
        }
    };

            // Create trending books
            var trendingBooks = new List<TrendingBook>
    {
        new TrendingBook
        {
            Id = 1,
            BookId = 1,
            Book = books[0],
            AddedDate = DateTime.UtcNow.AddDays(-1)
        },
        new TrendingBook
        {
            Id = 2,
            BookId = 2,
            Book = books[1],
            AddedDate = DateTime.UtcNow.AddDays(-3)
        },
                new TrendingBook
        {
            Id = 3,
            BookId = 2,
            Book = books[1],
            AddedDate = DateTime.UtcNow.AddDays(-5)
        }
    };

            // Add data to context
            _context.Genres.AddRange(genres);
            _context.Authors.AddRange(authors);
            _context.Users.AddRange(users); // Add users
            _context.Books.AddRange(books);
            _context.TrendingBooks.AddRange(trendingBooks); // Add trending books
            _context.SaveChanges();
        }


    }
}
