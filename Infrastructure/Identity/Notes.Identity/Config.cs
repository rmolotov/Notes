// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Notes.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("Notes.Api"),
            new("Notes.Web"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new()
            {
                ClientId = "Client.Api",
                ClientName = "API client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "Notes.Api",
                    "Notes.Web"
                }
            },

            new()
            {
                ClientId = "Client.Web",
                ClientName = "Web Client",

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                RequireClientSecret = false,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "Notes.Web"
                }
            },
        };
}