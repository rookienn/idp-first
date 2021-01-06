using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices1(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                //默认验证方案
                options.DefaultScheme = "Cookies";
                //默认token验证失败后的确认验证结果方案
                options.DefaultChallengeScheme = "oidc";
            })
            //先添加一个名为Cookies的Cookie认证方案
            .AddCookie("Cookies")
            //添加OpenIdConnect认证方案
            .AddOpenIdConnect("oidc", options =>
            {
                //options.Authority = "http://localhost:5000";
                //options.RequireHttpsMetadata = false;
                //options.ClientId = "apiClientCode";
                //options.ClientSecret = "apiSecret";
                //options.ResponseType = "code";
                //options.Scope.Add("scope1"); //添加授权资源
                //options.SaveTokens = true;
                //options.GetClaimsFromUserInfoEndpoint = true;

                //指定远程认证方案的本地登录处理方案
                options.SignInScheme = "Cookies";
                //通过网关访问鉴权中心
                options.Authority = "https://localhost:5555";
                //Https强制要求标识
                options.RequireHttpsMetadata = false;
                //客户端ID（支持隐藏模式和授权码模式，密码模式和客户端模式不需要用户登录）
                //使用隐藏模式
                options.ClientId = "apiClientImpl";
                //options.ClientId = "apiClientCode";                
                options.ClientSecret = "apiSecret";
                //令牌保存标识
                options.SaveTokens = true;
                //添加访问secretapi域api的权限，用于access_token
                options.Scope.Add("scope1");
                //请求授权用户的PhoneModel Claim，随id_token返回
                options.Scope.Add("PhoneModel");
                options.Scope.Add("openid");

                //请求返回id_token以及token
                options.ResponseType = OpenIdConnectResponseType.IdTokenToken;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            //});

            services.ConfigureNonBreakingSameSiteCookies();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = "http://localhost:5004";
                options.RequireHttpsMetadata = false;
                options.ClientId = "blogadminjs";
                options.ClientSecret = "12345678";
                options.SaveTokens = true;
                options.Scope.Add("blog.core.api");
                options.Scope.Add("roles");
                options.Scope.Add("profile");
                options.Scope.Add("openid");
                options.ResponseType = OpenIdConnectResponseType.IdTokenToken;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.ConfigureNonBreakingSameSiteCookies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();
            
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
