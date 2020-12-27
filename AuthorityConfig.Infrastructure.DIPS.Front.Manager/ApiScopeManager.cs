using AuthorityConfig.Domain.Exceptions;
using AuthorityConfig.Domain.Param;
using AuthorityConfig.Specification.Business;
using AuthorityConfig.Specification.Repository;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorityConfig.Infrastructure.DIPS.Front.Managers
{
    public class ApiScopeManager : BaseManager, IApiScopeManager
    {
        public ApiScopeManager(IAuthorityRepository authorityRepository) : base(authorityRepository) { }

        public async Task<IEnumerable<ApiScope>> GetApiScopesAsync(AuthorityParam param, CancellationToken cancellationToken)
        {
            var config = await GetConfigurationAsync(param.Authority, cancellationToken);
            if (config == null)
            {
                throw new AuthorityDoesNotExistsException(param.Authority);
            }

            return config.Apis.ToArray();
        }

        public async Task AddApiScopeAsync(AddApiParam param, CancellationToken cancellationToken)
        {
            var config = await GetConfigurationAsync(param.Authority, cancellationToken);
            if (config == null)
            {
                throw new AuthorityDoesNotExistsException(param.Authority);
            }

            var api = config.Apis == null ? null : config.Apis.Where(a => a.Name.Equals(param.Name)).FirstOrDefault();
            if (api != null)
            {
                throw new ApiScopeExistsException(param.Name);
            }

            api = new ApiScope
            {
                Name = param.Name,
                DisplayName = string.IsNullOrWhiteSpace(param.DisplayName) ? param.Name : param.DisplayName
            };

            var newApis = new List<ApiScope>();
            if (config.Apis != null) newApis.AddRange(config.Apis);
            newApis.Add(api);
            config.Apis = newApis;

            await SetConfigurationAsync(authority: param.Authority, config: config, cancellationToken: cancellationToken);
        }

    }

}
