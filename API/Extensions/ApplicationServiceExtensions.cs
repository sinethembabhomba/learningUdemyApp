using API.Data;
using API.helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
      public static IServiceCollection AddApplicationServices(this IServiceCollection services,
      IConfiguration config)
      {
        services.AddDbContext<DataContext>(opt => {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });

        services.AddCors();
        services.AddScoped<ITokenSrvice, TokenService>();
        services.AddScoped<IUserRepository,UserRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService,PhotoService>();
        services.AddScoped<LogUserActivity>();
        services.AddScoped<ILikesRepository,LikesRepository>();
        services.AddScoped<IMessageRepository,MessageRepository>();
        services.AddSignalR();
        return services;
      }  
    }
}