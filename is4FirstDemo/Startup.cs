// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityServer4.Services;
using is4FirstDemo.IdentityUserStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace is4FirstDemo
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfigurationRoot Configuration { get; }


        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)//增加环境配置文件，新建项目默认有
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //        .SetBasePath(Directory.GetCurrentDirectory())
            //        .AddXmlFile("App.config", optional: false).Build();
            //services.AddSingleton(configuration);
            //var conn = configuration.GetConnectionString("MyConn");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connection = Configuration.GetConnectionString("MySqlDbConnectString");

            
            services.AddControllersWithViews();
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var builder = services.AddIdentityServer()
                //.AddDeveloperSigningCredential()
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = context =>
                    {
                        context.UseMySql(connection, sql =>
                        {
                            sql.ServerVersion(new ServerVersion(new Version(8, 0, 22), ServerType.MySql));
                            sql.MigrationsAssembly(migrationsAssembly);
                        });
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseMySql(connection,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })
                .AddUserStore(opt =>
                {
                    opt.UseMySql(connection, sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                    });
                });

            //if (Environment.IsDevelopment())
            //{
            builder.AddDeveloperSigningCredential();
            //}
            //else
            //{
            //builder.AddSigningCredential(new X509Certificate2(Path.Combine(Environment.ContentRootPath,
            //         Configuration["Certificates:CertPath"]),
            //         Configuration["Certificates:Password"]));
            //}


            //builder.Services.AddTransient<IProfileService, ProfileService>();

            //services.AddDbContext<UserStoreDbContext>(opt => {
            //    opt.UseMySql(Configuration.GetConnectionString("MySqlDbConnectString"));
            //});
            //services.AddIdentity<CustomIdentityUser, IdentityRole>()
            //  .AddEntityFrameworkStores<UserStoreDbContext>()
            //  .AddDefaultTokenProviders();

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            //});
            services.ConfigureNonBreakingSameSiteCookies();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        /// <summary>
        /// 内存
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureServices1(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(Config.GetTestUsers);

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddDirectoryBrowser();

        }

        public void Configure(IApplicationBuilder app)
        {
            //InitializeDatabase(app);

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCookiePolicy();

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthentication();
            // uncomment, if you want to add MVC
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="app"></param>
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                #region 迁移用户
                //dotnet ef migrations add InitialIdentityServerIdentityUserDbMigration - c UserStoreDbContext - o Data / Migrations / IdentityServer / IdentityUserDb
                //dotnet ef database update -c UserStoreDbContext
                //serviceScope.ServiceProvider.GetRequiredService<UserStoreDbContext>().Database.Migrate();

                //var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<UserStoreDbContext>();

                //IdentityRole role = null;
                //if (!applicationDbContext.Roles.Any())
                //{
                //    role = new IdentityRole()
                //    {
                //        Name = "admin",
                //        NormalizedName = "admin"
                //    };
                //    applicationDbContext.Roles.Add(role);
                //}
                //else
                //{
                //    role = applicationDbContext.Roles.Where(r => r.Name.Equals("Admin")).SingleOrDefault();
                //}
                //if (!applicationDbContext.Users.Any())
                //{
                //    var user = new CustomIdentityUser()
                //    {
                //        UserName = "administrator",
                //        PasswordHash = "admin123456".Sha256(),
                //        Email = "haha@xx.com",
                //        NormalizedUserName = "admin"
                //    };
                //    applicationDbContext.UserClaims.Add(new IdentityUserClaim<string>()
                //    {
                //        ClaimType = ClaimTypes.Country,
                //        ClaimValue = "CSC",
                //        UserId = user.Id
                //    });
                //    applicationDbContext.Set<CustomIdentityUser>().Add(user);
                //    if (role != null)
                //    {
                //        applicationDbContext.UserRoles.Add(new IdentityUserRole<string>()
                //        {
                //            RoleId = role.Id,
                //            UserId = user.Id
                //        });
                //    }

                //}
                //applicationDbContext.SaveChanges(); 
                #endregion

                var userContext = serviceScope.ServiceProvider.GetRequiredService<UserStoreDbContext1>();
                //添加config中的Users数据到数据库
                if (!userContext.IdentityUser.Any())
                {
                    int index = 0;
                    foreach (var user in Config.GetTestUsers)
                    {
                        IdentityUser1 iuser = new IdentityUser1()
                        {
                            IsActive = user.IsActive,
                            Password = user.Password,
                            ProviderName = user.ProviderName,
                            ProviderSubjectId = user.ProviderSubjectId,
                            SubjectId = user.SubjectId,
                            Username = user.Username,
                            IdentityUserClaims = user.Claims.Select(r => new IdentityUserClaim1()
                            {
                                ClaimId = (index++).ToString(),
                                Name = r.Type,
                                Value = r.Value
                            }).ToList()
                        };
                        userContext.IdentityUser.Add(iuser);
                    }
                    userContext.SaveChanges();
                }


                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                //var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                //context.Database.Migrate();
                //if (!context.Clients.Any())
                //{
                //    foreach (var client in Config.Clients)
                //    {
                //        context.Clients.Add(client.ToEntity());
                //    }
                //    context.SaveChanges();
                //}

                //if (!context.IdentityResources.Any())
                //{
                //    foreach (var resource in Config.IdentityResources)
                //    {
                //        context.IdentityResources.Add(resource.ToEntity());
                //    }
                //    context.SaveChanges();
                //}

                //if (!context.ApiResources.Any())
                //{
                //    foreach (var resource in Config.GetApis())
                //    {
                //        context.ApiResources.Add(resource.ToEntity());
                //    }
                //    context.SaveChanges();
                //}

                //if (!context.ApiScopes.Any())
                //{
                //    foreach (var apiScope in Config.ApiScopes)
                //    {
                //        context.ApiScopes.Add(apiScope.ToEntity());
                //    }
                //    context.SaveChanges();
                //}
            }
        }
    }
}
