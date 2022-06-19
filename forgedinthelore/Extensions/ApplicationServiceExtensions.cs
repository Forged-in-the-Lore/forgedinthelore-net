using forgedinthelore_net.Data;
using forgedinthelore_net.Helpers;
using forgedinthelore_net.Interfaces;
using forgedinthelore_net.Services;
using Microsoft.EntityFrameworkCore;

namespace forgedinthelore_net.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // services.AddSingleton<PresenceTracker>(); - this is for websocket, currently disables cause not implemented

        services.AddAutoMapper(typeof(AutomapperProfiles).Assembly);

        //AddScoped means that the service exists for the duration of the HTTP request
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITokenCreatorService, TokenCreatorService>();
        services.AddScoped<ITokenValidatorService, TokenValidatorService>();


        //Add the database context
        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        });

        return services;
    }
}