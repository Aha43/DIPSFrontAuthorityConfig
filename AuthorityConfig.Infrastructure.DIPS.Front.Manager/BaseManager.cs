using AuthorityConfig.Infrastructure.DIPS.Front.Managers.Domain;
using AuthorityConfig.Specification.Repository;
using AuthorityConfig.Specification.Repository.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorityConfig.Infrastructure.DIPS.Front.Managers
{
    public abstract class BaseManager
    {
        protected readonly IAuthorityRepository _authorityRepository;

        public BaseManager(
            IAuthorityRepository authorityRepository)
        {
            _authorityRepository = authorityRepository;
        }

        protected async Task<IdserverConfig> GetConfigurationAsync(string authority, CancellationToken cancellationToken)
        {
            var stored = await _authorityRepository.GetConfigurationAsync(authority, cancellationToken);
            if (stored == null)
            {
                return null;
            }
            var retVal = JsonSerializer.Deserialize<IdserverConfig>(stored.Json);
            return retVal;
        }

        protected async Task SetConfigurationAsync(string authority, CancellationToken cancellationToken, IdserverConfig config = null, string uri = null, string description = null)
        {
            var dao = new AuthorityDao
            {
                Authority = authority,
                Json = (config == null) ? null : JsonSerializer.Serialize(config),
                Uri = uri,
                Description = description
            };

            await _authorityRepository.SetConfigurationAsync(dao, cancellationToken);
        }

    }

}
