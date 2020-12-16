using AuthorityConfig.Domain.Exceptions;
using AuthorityConfig.Domain.Param;
using AuthorityConfig.Infrastructure.DIPS.Front.Managers.Domain;
using AuthorityConfig.Specification.Business;
using AuthorityConfig.Specification.Repository;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorityConfig.Infrastructure.DIPS.Front.Managers
{
    public class ClientManager : BaseManager, IClientManager
    {
        public ClientManager(IAuthorityRepository authorityRepository) : base(authorityRepository) { }

        public async Task<IEnumerable<Client>> GetClientsAsync(AuthorityParam param, CancellationToken cancellationToken)
        {
            var config = await GetConfigurationAsync(param.Authority, cancellationToken);
            if (config == null)
            {
                throw new AuthorityDoesNotExists(param.Authority);
            }

            return config.Clients.ToArray();
        }

        public async Task SetClientAsync(SetClientParam param, CancellationToken cancellationToken)
        {
            var config = await GetConfigurationAsync(param.Authority, cancellationToken);
            if (config == null)
            {
                throw new AuthorityDoesNotExists(param.Authority);
            }

            var client = GetClient(config, param);

            AddScopes(client, param);
            // ToDo: More modificators

            await SetConfigurationAsync(authority: param.Authority, config: config, cancellationToken: cancellationToken);
        }

        #region set_client_support
        private Client GetClient(IdserverConfig config, SetClientParam param)
        {
            var retVal = config.Clients.Where(c => c.ClientId.Equals(param.ClientId)).FirstOrDefault();
            if (retVal == null)
            {
                return CreateClient(config, param);
            }

            return retVal;
        }

        private Client CreateClient(IdserverConfig config, SetClientParam param)
        {
            throw new NotImplementedException("not yet...");
        }

        private void AddScopes(Client client, SetClientParam param)
        {
            var scopesToAdd = param.ScopesToAdd;
            if (!string.IsNullOrWhiteSpace(scopesToAdd))
            {
                var scopesToAddSet = new HashSet<string>(scopesToAdd.Split(' '));
                var existingScopes = new HashSet<string>(client.AllowedScopes == null ? new string[] { } : client.AllowedScopes);
                var newScopes = existingScopes.Union(scopesToAddSet);
                client.AllowedScopes = newScopes.ToArray();
            }
        }
        #endregion

    }

}
