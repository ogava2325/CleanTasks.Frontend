using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Services.External;

namespace Services.Extension;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        const string baseUrl = "https://localhost:7021/";
        
        services.AddRefitClient<IUserService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<IRoleService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
        
        return services;
    }
}