using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class BookProxyService : IBookProxyService
    {
        private readonly IBookRepository _bookRepository;
        //                     bookId  rate
        public static Dictionary<int, decimal?> BooksRates { get; set; } = new();
        public BookProxyService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public async Task<decimal?> CalculateRate(int bookId)
        {
            if(BooksRates.TryGetValue(bookId, out var rate))
                return rate;
            
            var rates = await _bookRepository.GetBookRates(bookId);

            if (rates.IsNullOrEmpty())
                return null;

            rate = ((rates.Sum() / (rates.Count * 10)) * 10);

            BooksRates.Add(bookId, rate);
            return rate;
        }
        public async Task UpdateRate(int bookId, decimal newRate)
        {
            if (BooksRates.TryGetValue(bookId, out var rate) == false)
                return;
            var reviewsCountBeforeAddingNewReview = (await _bookRepository.GetBookReviewsCount(bookId)) - 1;
            if(reviewsCountBeforeAddingNewReview == 0)
            {
                BooksRates.Remove(bookId);
                return;
            }
            decimal? updatedRate = ((rate * reviewsCountBeforeAddingNewReview) + newRate) / (reviewsCountBeforeAddingNewReview + 1);
            BooksRates[bookId] = updatedRate;
        }
    }
}
