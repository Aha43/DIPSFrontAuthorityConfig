using AuthorityConfig.Domain.Param;
using AuthorityConfig.Domain.Response;
using AuthorityConfig.Specification.Business;
using AuthorityConfig.Specification.Repository;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorityConfig.Infrastructure.Default.Managers
{
    public class ApiScopeManager : IApiScopeManager
    {
        private readonly IAuthorityRepository _repository;

        public ApiScopeManager(IAuthorityRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiScope> GetApiScopeAsync(GatApiScopeParam param, CancellationToken cancellationToken)
        {
            return await _repository.GetApiScopeAsync(param, cancellationToken);
        }

        public async Task<IEnumerable<ApiScope>> GetApiScopesAsync(AuthorityParam param, CancellationToken cancellationToken)
        {
            return await _repository.GetApiScopesAsync(param, cancellationToken);
        }

        public async Task<SetApiScopeResponse> SetApiScopeAsync(SetApiParam param, CancellationToken cancellationToken)
        {
            var retVal = new SetApiScopeResponse();

            var apiScope = await _repository.GetApiScopeAsync(new GatApiScopeParam
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

                retVal.Created = true;
            }

            if (!string.IsNullOrEmpty(param.DisplayName)) apiScope.DisplayName = param.DisplayName;
            
            if (!param.DryRun) await _repository.SetApiScopeAsync(apiScope, param, cancellationToken);

            retVal.ApiScope = apiScope;
            retVal.DryRun = param.DryRun;
            return retVal;
        }

    }

}
