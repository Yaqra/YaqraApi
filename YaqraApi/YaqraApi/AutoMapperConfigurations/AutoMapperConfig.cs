using AutoMapper;
using YaqraApi.DTOs.Auth;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.ReadingGoal;
using YaqraApi.DTOs.User;
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
            });

            return new Mapper(config);
        }
    }
}
