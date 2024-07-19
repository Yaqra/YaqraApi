using AutoMapper;
using YaqraApi.DTOs.Auth;
using YaqraApi.DTOs.Author;
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
            });

            return new Mapper(config);
        }
    }
}
