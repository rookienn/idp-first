using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                //IdentityServer地址
                options.Authority = "http://localhost:5000";
                //对应Idp中ApiResource的Name
                options.Audience = "secretapi";
                //不使用https
                options.RequireHttpsMetadata = false;
            });


            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //services.AddAuthentication("Bearer").AddJwtBearer(r => {
            //    //认证地址
            //    r.Authority = "http://localhost:5000";
            //    //权限标识
            //    r.Audience = "secretapi";
            //    //是否必需HTTPS
            //    r.RequireHttpsMetadata = false;
            //});


            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //.AddIdentityServerAuthentication("orderService", options =>
            //{
            //    options.Authority = "http://localhost:5000";//鉴权中心地址
            //    options.ApiName = "secretapi";
            //    options.SupportedTokens = SupportedTokens.Both;
            //    //options.ApiSecret = "orderApi secret";
            //    options.RequireHttpsMetadata = false;
            //});
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapControllers();
            });
        }
    }
}
