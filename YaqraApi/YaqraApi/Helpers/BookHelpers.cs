using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Genre;
using YaqraApi.Models;

namespace YaqraApi.Helpers
{
    public static class BookHelpers
    {
        private static Mapper _mapper = AutoMapperConfig.InitializeAutoMapper();
        public static ICollection<BookDto> ConvertBooksToBookDtos(ICollection<Book> books)
        {
            var result = new List<BookDto>();
            foreach (var book in books)
            {
                var dto = _mapper.Map<BookDto>(book);
                dto = AddAuthorsToBookDto(dto, book.Authors);
                dto = AddGenresToBookDto(dto, book.Genres);
                result.Add(dto);
            }
            return result;
        }
        private static BookDto AddAuthorsToBookDto(BookDto dto,ICollection<Author> authors)
        {
            foreach (var author in authors)
                dto.AuthorsDto.Add(_mapper.Map<AuthorDto>(author));
            return dto;
        }
        private static BookDto AddGenresToBookDto(BookDto dto, ICollection<Genre> genres)
        {
            foreach (var genre in genres)
                dto.GenresDto.Add(new GenreDto { GenreId = genre.Id, GenreName = genre.Name});
            return dto;
        }
        public static decimal? CalcualteRate(List<decimal>? rates)
        {
            if (rates.IsNullOrEmpty())
                return null;
            var sum = rates.Sum();
            return ((sum / (rates.Count * 10)) * 10);
        }
        public static string? FormatRate(decimal? rate)
        {
            if (rate == null)
                return null;
            return rate.Value.ToString("0.00");
        }
    }
}
