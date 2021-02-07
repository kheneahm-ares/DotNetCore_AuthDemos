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
        //we tell identity server to support openid 
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "tech.scope",
                    UserClaims =
                    {
                        "tech.read"
                    }
                }
            };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {
                new ApiResource("ApiOne", new string[] {"tech.write"}) { Scopes = {new Scope("scope_one:read")} },
                new ApiResource("ApiTwo") { Scopes = {new Scope("scope_two:read")} }}; //register one

        public static IEnumerable<Client> GetClients() =>
            new List<Client> { new Client
            {
                ClientId = "client_id",
                ClientSecrets = { new Secret("client_secret".ToSha256()) },

                AllowedGrantTypes = GrantTypes.ClientCredentials, //how to retrieve tokens,
                AllowedScopes = { "scope_one:read"}, //what can this client access
                RequireConsent = false
            },
                 new Client
            {
                ClientId = "client_id_mvc",
                ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },

                AllowedGrantTypes = GrantTypes.Code, //how to retrieve tokens,
                AllowedScopes = { "scope_one:read", "scope_two:read", "openid", "profile", "tech.scope" }, //what can this client access
                RedirectUris = {"https://localhost:44306/signin-oidc" }, //necessary for Auth Code flow, for client, we know it's using oidc, and this is the default redirect uri set up by oidc
                RequireConsent = false,
                AllowOfflineAccess = true


                //puts all user claims in id token
                //AlwaysIncludeUserClaimsInIdToken = true
                     
            },
                                  new Client
            {
                ClientId = "client_id_js",

                AllowedGrantTypes = GrantTypes.Implicit, //how to retrieve tokens,
                AllowedScopes = { "scope_one:read", "scope_two:read", "openid", "profile", "tech.scope" }, //what can this client access
                RedirectUris = {"https://localhost:44321/Home/SignIn" }, //what we specified in our js client
                RequireConsent = false,
                AllowAccessTokensViaBrowser = true,
                AllowedCorsOrigins = { "https://localhost:44321" }


                     
            }
            };

        
    }
}
