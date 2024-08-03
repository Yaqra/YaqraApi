using AutoMapper;
using YaqraApi.DTOs.Auth;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Community;
using YaqraApi.DTOs.ReadingGoal;
using YaqraApi.DTOs.User;
using YaqraApi.DTOs.UserBookWithStatus;
using YaqraApi.Models;

namespace YaqraApi.AutoMapperConfigurations
{
    public class AutoMapperConfig
    {
        public static Mapper InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                //user userDto
                cfg.CreateMap<UserDto, ApplicationUser>()
                    .ForMember(dest => dest.Id, act => act.MapFrom(src => src.UserId))
                    .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Username))
                    .ForMember(dest => dest.Bio, act => act.MapFrom(src => src.Bio))
                    .ForMember(dest => dest.ProfilePicture, act => act.MapFrom(src => src.ProfilePicture))
                    .ForMember(dest => dest.ProfileCover, act => act.MapFrom(src => src.ProfileCover))
                    .ReverseMap();

                //author authorDto
                cfg.CreateMap<AuthorDto, Author>()
                    .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Bio, act => act.MapFrom(src => src.Bio))
                    .ForMember(dest => dest.Picture, act => act.MapFrom(src => src.Picture))
                    .ReverseMap();

                //author authorWithoutPicDto
                cfg.CreateMap<AuthorWithoutPicDto, Author>()
                    .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Bio, act => act.MapFrom(src => src.Bio))
                    .ReverseMap();


                //authorDto authorWithoutPicDto
                cfg.CreateMap<AddAuthorDto, AuthorDto>()
                    .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Bio, act => act.MapFrom(src => src.Bio))
                    .ReverseMap();

                //readingGoal readingGoalDto
                cfg.CreateMap<ReadingGoalDto, ReadingGoal>()
                    .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
                    .ForMember(dest => dest.NumberOfBooksToRead, act => act.MapFrom(src => src.NumberOfBooksToRead))
                    .ForMember(dest => dest.StartDate, act => act.MapFrom(src => src.StartDate))
                    .ForMember(dest => dest.DurationInDays, act => act.MapFrom(src => src.DurationInDays))
                    .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Description))
                    .ForMember(dest => dest.Title, act => act.MapFrom(src => src.Title))
                    .ForMember(dest => dest.UserId, act => act.MapFrom(src => src.UserId))
                    .ReverseMap();

                //readingGoalDto addReadingGoalDto
                cfg.CreateMap<AddReadingGoalDto, ReadingGoalDto> ()
                    .ForMember(dest => dest.NumberOfBooksToRead, act => act.MapFrom(src => src.NumberOfBooksToRead))
                    .ForMember(dest => dest.StartDate, act => act.MapFrom(src => src.StartDate))
                    .ForMember(dest => dest.DurationInDays, act => act.MapFrom(src => src.DurationInDays))
                    .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Description))
                    .ForMember(dest => dest.Title, act => act.MapFrom(src => src.Title))
                    .ReverseMap();

                //book bookDto
                cfg.CreateMap<BookDto, Book>()
                    .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, act => act.MapFrom(src => src.Title))
                    .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Description))
                    .ForMember(dest => dest.Image, act => act.MapFrom(src => src.Image))
                    .ForMember(dest => dest.AddedDate, act => act.MapFrom(src => src.AddedDate))
                    .ForMember(dest => dest.NumberOfPages, act => act.MapFrom(src => src.NumberOfPages))
                    .ReverseMap();

                //bookDto bookWithoutImageDto
                cfg.CreateMap<BookDto, BookWithoutImageDto>()
                    .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, act => act.MapFrom(src => src.Title))
                    .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Description))
                    .ForMember(dest => dest.AddedDate, act => act.MapFrom(src => src.AddedDate))
                    .ForMember(dest => dest.NumberOfPages, act => act.MapFrom(src => src.NumberOfPages))
                    .ReverseMap();

                //userBook userBookDto 
                cfg.CreateMap<UserBookWithStatus, UserBookWithStatusDto>()
                    .ForMember(dest => dest.UserId, act => act.MapFrom(src => src.UserId))
                    .ForMember(dest => dest.Status, act => act.MapFrom(src => src.Status))
                    .ForMember(dest => dest.BookId, act => act.MapFrom(src => src.BookId))
                    .ForMember(dest => dest.AddedDate, act => act.MapFrom(src => src.AddedDate))
                    .ReverseMap();

                //community
                cfg.CreateMap<Post, PostDto>()
                   .Include<Review, ReviewDto>()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                   .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                   .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                   .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.LikeCount))
                   .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                   .ReverseMap();

                cfg.CreateMap<Review, ReviewDto>()
                   .IncludeBase<Post, PostDto>()
                   .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                   .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                   .ReverseMap();

                //addreviewdto  reviewdto
                cfg.CreateMap<ReviewDto, AddReviewDto>()
                   .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                   .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                   .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                   .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                   .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                   .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                   .ReverseMap();
            });

            return new Mapper(config);
        }
    }
}
