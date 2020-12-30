using AuthorityConfig.Specification.Business;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorityConfig.Infrastructure.Default.Managers.Config
{
    public static class IoCConfig
    {
        public static IServiceCollection ConfigureManagerServices(this IServiceCollection services)
        {
            return services.AddSingleton<IAuthorityManager, AuthorityManager>()
                .AddSingleton<IClientManager, ClientManager>()
                .AddSingleton<IApiScopeManager, ApiScopeManager>();
        }
    }
}
