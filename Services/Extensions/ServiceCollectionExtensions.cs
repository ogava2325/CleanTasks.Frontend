using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using services.External;

namespace services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRefitServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var baseUrl = configuration["BaseUrl"];
        
        services.AddRefitClient<IUserService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<IRoleService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<IProjectService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<IColumnService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<ICardService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<IStateService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<ICommentService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<IStatsService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
        
        return services;
    }
}