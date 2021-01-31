using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Configuration
    {
        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> { new ApiResource("ApiOne") { Scopes = {new Scope("scope_one:read")} } }; //register one

        public static IEnumerable<Client> GetClients() =>
            new List<Client> { new Client
            {
                ClientId = "client_id",
                ClientSecrets = { new Secret("client_secret".ToSha256()) },

                AllowedGrantTypes = GrantTypes.ClientCredentials, //how to retrieve tokens,
                AllowedScopes = { "scope_one:read"} //what can this client access
            } 
            };

        
    }
}
