using Microsoft.Extensions.DependencyInjection;

namespace DemoApp.Application.Extensions;

public static class MapperCollectionExtensions
{
    public static IServiceCollection AddApplicationMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        { 
            cfg.CreateMap<Domain.Entities.User, Application.DTO.UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        });
        return services;
    }
}
