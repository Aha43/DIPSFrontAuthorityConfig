using AuthorityConfig.Domain.Param;
using AuthorityConfig.Specification.Business;
using AuthorityConfig.Specification.Repository;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorityConfig.Infrastructure.DIPS.Front.Managers
{
    public class ApiScopeManager : BaseManager, IApiScopeManager
    {
        public ApiScopeManager(IAuthorityRepository authorityRepository) : base(authorityRepository) { }

        public async Task<ApiScope> GetApiScopeAsync(GatApiScopeParam param, CancellationToken cancellationToken)
        {
            return await _authorityRepository.GetApiScopeAsync(param, cancellationToken);
        }

        public async Task<IEnumerable<ApiScope>> GetApiScopesAsync(AuthorityParam param, CancellationToken cancellationToken)
        {
            return await _authorityRepository.GetApiScopesAsync(param, cancellationToken);
        }

        public async Task SetApiScopeAsync(SetApiParam param, CancellationToken cancellationToken)
        {
            var apiScope = await _authorityRepository.GetApiScopeAsync(new GatApiScopeParam
            {
                Authority = param.Authority,
                Name = param.Name
            }, cancellationToken);

            if (apiScope == null)
            {
                apiScope = new ApiScope
                {
                    Name = param.Name,
                    DisplayName = param.Name // ensure got display name if not set from param
                };
            }

            if (!string.IsNullOrEmpty(param.DisplayName)) apiScope.DisplayName = param.DisplayName;

            await _authorityRepository.SetApiScopeAsync(apiScope, param, cancellationToken);
        }

    }

}
