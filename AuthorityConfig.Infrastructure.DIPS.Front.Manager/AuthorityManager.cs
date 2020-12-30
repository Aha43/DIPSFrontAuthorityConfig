using AuthorityConfig.Domain.Exceptions;
using AuthorityConfig.Domain.Model;
using AuthorityConfig.Domain.Param;
using AuthorityConfig.Infrastructure.DIPS.Front.Managers.Domain;
using AuthorityConfig.Specification.Business;
using AuthorityConfig.Specification.Repository;
using AuthorityConfig.Specification.Repository.Dao;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorityConfig.Infrastructure.DIPS.Front.Managers
{
    public class AuthorityManager : BaseManager, IAuthorityManager
    {
        public AuthorityManager(IAuthorityRepository authorityRepository) : base(authorityRepository) { }

        public async Task<object> GetConfigurationAsync(AuthorityParam param, CancellationToken cancellationToken)
        {
            return await _authorityRepository.GetConfigurationAsync(param, cancellationToken);
        }

        public async Task SetConfigurationAsync(SetConfigParam param, CancellationToken cancellationToken)
        {
            //var dao = new AuthorityDao
            //{
            //    Authority = param.Authority,
            //    Json = JsonSerializer.Serialize(param.Config),
            //    Uri = param.Uri,
            //    Description = param.Description
            //};

            await _authorityRepository.SetConfigurationAsync(param, cancellationToken);
        }

        public async Task<Authorities> GetAuthoritiesAsync(CancellationToken cancellationToken)
        {
            return await _authorityRepository.GetAuthoritiesAsync(cancellationToken);
        }

        public async Task<Authority> GetAuthorityAsync(GetAuthorityParam param, CancellationToken cancellationToken)
        {
            return await _authorityRepository.GetAuthorityAsync(param, cancellationToken);
        }

    }

}
