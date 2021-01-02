using AuthorityConfig.Domain.Exceptions;
using AuthorityConfig.Domain.Param;
using AuthorityConfig.Domain.Response;
using AuthorityConfig.Specification.Business;
using AuthorityConfig.Specification.Repository;
using AuthorityConfig.Utility;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static AuthorityConfig.Domain.Oidc.OidcConstants;

namespace AuthorityConfig.Infrastructure.Default.Managers
{
    public class ClientManager : IClientManager
    {
        private readonly IAuthorityRepository _repository;

        public ClientManager(IAuthorityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Client>> GetClientsAsync(AuthorityParam param, CancellationToken cancellationToken)
        {
            return await _repository.GetClientsAsync(param, cancellationToken);
        }

        public async Task<SetClientResponse> SetClientAsync(SetClientParam param, CancellationToken cancellationToken)
        {
            var retVal = await GetClientAsync(param, cancellationToken);

            SetProperties(retVal.Client, param);
            SetClientSecret(retVal, param);

            retVal.Client.AllowedScopes = retVal.Client.AllowedScopes.UnionWithTokens(param.ScopesToAdd);
            retVal.Client.AllowedScopes = retVal.Client.AllowedScopes.RemoveTokens(param.ScopesToRemove);

            retVal.Client.AllowedGrantTypes = retVal.Client.AllowedGrantTypes.UnionWithTokens(param.GrantTypesToAdd);
            retVal.Client.AllowedGrantTypes = retVal.Client.AllowedGrantTypes.RemoveTokens(param.GrantTypesToRemove);
            if (retVal.Client.AllowedGrantTypes.Count == 0)
            {
                throw new NoAllowedGrantsGivenException();
            }

            if (!param.DryRun) await _repository.SetClientAsync(retVal.Client, param, cancellationToken);

            retVal.DryRun = param.DryRun;
            return retVal;
        }

        private async Task<SetClientResponse> GetClientAsync(SetClientParam param, CancellationToken cancellationToken)
        {
            var client = await _repository.GetClientAsync(new GetClientParam { Authority = param.Authority, ClientId = param.ClientId }, cancellationToken);

            if (client == null)
            {
                if (param.CreateIfDoesNotExists)
                {
                    var retVal = CreateClient(param);
                    return retVal;
                }
                else
                {
                    throw new ClientDoesNotExistsException(param.ClientId);
                }
            }
            
            return new SetClientResponse
            {
                Client = client
            };
        }

        private static SetClientResponse CreateClient(SetClientParam param)
        {
            if (!string.IsNullOrWhiteSpace(param.GrantTypesToRemove))
            {
                throw new InvalidParamException("Grant types to remove given when client do not exists and is to be created");
            }
            if (!string.IsNullOrWhiteSpace(param.ScopesToRemove))
            {
                throw new InvalidParamException("Scopes to remove given when client do not exists and is to be created");
            }
            if (string.IsNullOrWhiteSpace(param.GrantTypesToAdd))
            {
                throw new InvalidParamException("No grant types to add given when client do not exists and is to be created");
            }

            try
            {
                var client = new Client()
                {
                    ClientId = param.ClientId,
                    ProtocolType = Protocol.Name
                };

                return new SetClientResponse
                {
                    Created = true,
                    Client = client
                };
            }
            catch (Exception ex)
            {
                throw new FailedToCreateClientException(ex);
            }
        }

        private static void SetClientSecret(SetClientResponse clientResponse, SetClientParam param)
        {
            var client = clientResponse.Client;

            Secret clientSecret = null;

            if (!string.IsNullOrEmpty(param.ClientSecret))
            {
                clientSecret = new Secret
                {
                    Value = param.ClientSecret.Sha256()
                };

                clientResponse.Secret = param.ClientSecret;
            }
            else if (clientResponse.Client.RequireClientSecret)
            {
                var (plain, hash) = SecretGenerator.GenerateSharedSecret();
                clientSecret = new Secret
                {
                    Type = "SharedSecret",
                    Value = hash,
                    Description = "SharedSecret for client " + (client.ClientName ?? client.ClientId)
                };

                clientResponse.Secret = plain;
            }

            if (clientSecret != null)
            {
                clientSecret.Type = "SharedSecret";
                clientSecret.Description = "SharedSecret for client " + (client.ClientName ?? client.ClientId);

                if (client.ClientSecrets == null) client.ClientSecrets = new List<Secret>();
                client.ClientSecrets.Add(clientSecret);
            }
        }

        private static void SetProperties(Client client, SetClientParam param)
        {
            if (!string.IsNullOrEmpty(param.ClientName)) client.ClientName = param.ClientName;
            if (!string.IsNullOrEmpty(param.Description)) client.Description = param.Description;
            if (!string.IsNullOrEmpty(param.ClientUri)) client.ClientUri = param.ClientUri;
            if (!string.IsNullOrEmpty(param.LogoUri)) client.LogoUri = param.LogoUri;
            if (param.Enabled != null) client.Enabled = param.Enabled.Value;
            if (param.RequireClientSecret != null) client.RequireClientSecret = param.RequireClientSecret.Value;
            if (param.RequireConsent != null) client.RequireConsent = param.RequireConsent.Value; // research
            if (param.AllowRememberConsent != null) client.AllowRememberConsent = param.AllowRememberConsent.Value; // research
        }

    }

}
