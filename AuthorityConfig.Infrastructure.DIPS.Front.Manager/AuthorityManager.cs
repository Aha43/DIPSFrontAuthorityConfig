using AuthorityConfig.Domain.Model;
using AuthorityConfig.Domain.Param;
using AuthorityConfig.Specification.Business;
using AuthorityConfig.Specification.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorityConfig.Infrastructure.Default.Managers
{
    public class AuthorityManager : IAuthorityManager
    {
        private readonly IAuthorityRepository _repository;

        public AuthorityManager(IAuthorityRepository repository)
        {
            _repository = repository;
        }

        public async Task<object> GetConfigurationAsync(AuthorityParam param, CancellationToken cancellationToken)
        {
            return await _repository.GetConfigurationAsync(param, cancellationToken);
        }

        public async Task SetConfigurationAsync(SetConfigParam param, CancellationToken cancellationToken)
        {
            await _repository.SetConfigurationAsync(param, cancellationToken);
        }

        public async Task<Authorities> GetAuthoritiesAsync(CancellationToken cancellationToken)
        {
            return await _repository.GetAuthoritiesAsync(cancellationToken);
        }

        public async Task<Authority> GetAuthorityAsync(GetAuthorityParam param, CancellationToken cancellationToken)
        {
            return await _repository.GetAuthorityAsync(param, cancellationToken);
        }

    }

}
