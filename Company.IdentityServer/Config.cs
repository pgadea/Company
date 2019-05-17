using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace Company.IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "pierre",
                    Password = "gadea"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "jenny",
                    Password = "password"
                }
            };
        }

        public static IEnumerable<ApiResource> GetAllApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("companyApi", "Customer Api for Company")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "companyApi" }
                },
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "companyApi" }
                },
                new Client
                {
                    ClientId = "swaggerapiui",
                    ClientName = "Swagger API UI",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = { "http://localhost:62838/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "http://localhost:62838/swagger" },

                    AllowedScopes = { "companyApi" },
                    AllowAccessTokensViaBrowser = true
                }
            };
        }
    }
}
