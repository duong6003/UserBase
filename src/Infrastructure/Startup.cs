using Hangfire;
using Hangfire.MySql;
using HangfireBasicAuthenticationFilter;
using Infrastructure.Mappings;
using Infrastructure.Modules.Users.Services;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructureConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServer(options => configuration.GetSection("HangfireSettings:Server").Bind(options));

        services.AddHangfire(hangfireConfiguration => hangfireConfiguration.UseStorage(
            new MySqlStorage(
                configuration["HangfireSettings:Storage:ConnectionString"],
                configuration.GetSection("HangfireSettings:Storage:Options").Get<MySqlStorageOptions>()
            )
        ));

        services.AddDbContextPool<ApplicationDbContext>(
            options => options.UseMySql(configuration["DatabaseSettings:MySQLSettings:ConnectionStrings:DefaultConnection"],
            ServerVersion.AutoDetect(configuration["DatabaseSettings:MySQLSettings:ConnectionStrings:DefaultConnection"])
        ));

        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

        services.AddAutoMapper(typeof(MappingProfile));

        #region Add Module Services
        services.AddScoped<IUserService, UserService>();
        #endregion

        return services;
    }

    public static IApplicationBuilder UseInfrastructureConfigure(this IApplicationBuilder app, IConfiguration configuration)
    {
        DashboardOptions dashboardOptions = configuration.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>();
        dashboardOptions.Authorization = new[]
        {
            new HangfireCustomBasicAuthenticationFilter
            {
                User = configuration["HangfireSettings:Credentials:Username"],
                Pass = configuration["HangfireSettings:Credentials:Password"]
            }
        };
        app.UseHangfireDashboard(configuration["HangfireSettings:Route"], dashboardOptions);

        return app;
    }
}
