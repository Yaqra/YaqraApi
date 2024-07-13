using AutoMapper;
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
                //user
                cfg.CreateMap<UserDto, ApplicationUser>()
                    .ForMember(dest => dest.Id, act => act.MapFrom(src => src.UserId))
                    .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Username))
                    .ForMember(dest => dest.Bio, act => act.MapFrom(src => src.Bio))
                    .ForMember(dest => dest.ProfilePicture, act => act.MapFrom(src => src.ProfilePicture))
                    .ForMember(dest => dest.ProfileCover, act => act.MapFrom(src => src.ProfileCover))
                    .ReverseMap();
            });

            return new Mapper(config);
        }
    }
}
