// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using static IdentityServer4.IdentityServerConstants;

namespace is4FirstDemo
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("PhoneModel","User's phone Model",new List<string>(){ "phonemodel","phoneprise","prog"})
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                 new ApiScope("scope1"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client()
                {
                     //客户端Id
                     ClientId="apiClientCd",
                     //客户端密码
                     ClientSecrets={new Secret("apiSecret".Sha256()) },
                     //客户端授权类型，ClientCredentials:客户端凭证方式
                     AllowedGrantTypes=GrantTypes.ClientCredentials,
                     //允许访问的资源
                     AllowedScopes=
                     {
                        "scope1"
                     }
                },
                new Client()
                {
                    ClientId="apiClientPassword",
                    ClientSecrets={ new Secret("apiSecret".Sha256()) },
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        "scope1"
                    }
                },
                new Client()
                { 
                    ClientId="apiClientImpl",
                    AllowedGrantTypes=GrantTypes.Implicit,
                    AllowedScopes =
                    {
                        "scope1",
                        "PhoneModel",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowAccessTokensViaBrowser=true,
                    RedirectUris={ "http://localhost:5002/auth_token.html","http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                    AlwaysIncludeUserClaimsInIdToken=true,
                    RequireConsent=true,
                },
                new Client()
                {
                     ClientId="apiClientCode",
                     ClientSecrets={new Secret("apiSecret".Sha256()) },
                     AllowedGrantTypes=GrantTypes.Code,
                     RedirectUris={ "http://localhost:5002/auth_code.html","http://localhost:5002/signin-oidc"},
                     AllowedScopes=
                     {
                        "scope1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                     },
                     AllowAccessTokensViaBrowser=true,
                     // 是否需要同意授权 （默认是false）
                     RequireConsent=true,
                     AllowOfflineAccess = true,
                     AllowRememberConsent = false,
                     AlwaysIncludeUserClaimsInIdToken=true,
                },
                new Client()
                {
                    AlwaysIncludeUserClaimsInIdToken = true,
                    ClientId = "apiClientHybrid",
                    ClientSecrets = { new Secret("apiSecret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris={ "http://localhost:5002/auth_token.html","http://localhost:5002/home/GetTokenData","http://localhost:5002/signin-oidc"},
                    AllowedScopes = { 
                        "scope1",
                        "PhoneModel",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true
                }
            };

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[] {
             //secretapi:标识名称，Secret Api：显示名称，可以自定义
                new ApiResource("secretapi","Secret Api")
                {
                    Scopes = { "scope1" }
                }
            };
        }

        public static List<TestUser> GetTestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "818727", Username = "alice", Password = "alice",
                    Claims=new List<Claim>(){
                        new Claim(ClaimTypes.Role,"admin"),
                        new Claim(ClaimTypes.Name,"alice"),
                        new Claim("prog","正式项目"),
                        new Claim("phonemodel","huawei"),
                        new Claim("phoneprise","5000元")
                    }
                },
                new TestUser
                {
                    SubjectId = "818728", Username = "guest", Password = "guest",
                    Claims = {
                        new Claim(JwtClaimTypes.Role,"guest")
                    }
                }
            };
    }
}