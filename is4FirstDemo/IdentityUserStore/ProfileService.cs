﻿using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace is4FirstDemo.IdentityUserStore
{
    public class ProfileService: IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.IssuedClaims.Count == 0)
            {
                if (context.Subject.Claims.Count() > 0)
                {
                    context.IssuedClaims = context.Subject.Claims.ToList();
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        { }
    }
}
