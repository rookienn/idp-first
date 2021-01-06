using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace is4FirstDemo.IdentityUserStore
{
    public static class IIdentityServerBuilderUserStoreExtensions1
    {
        public static IIdentityServerBuilder AddUserStore(this IIdentityServerBuilder builder, Action<DbContextOptionsBuilder> userStoreOptions = null)
        {
            builder.Services.AddDbContext<UserStoreDbContext1>(userStoreOptions);
            builder.Services.AddTransient<UserStore1>();
            return builder;
        }
    }
}
