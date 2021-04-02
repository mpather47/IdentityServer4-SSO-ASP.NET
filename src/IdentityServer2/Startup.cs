// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer2.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Reflection;

namespace IdentityServer2
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;


        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddDbContext<AppDbContext>(config =>
            {
            
                config.UseInMemoryDatabase("Memory");
            });

            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });


             services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryClients(Config.GetClients())
                .AddDeveloperSigningCredential();
             
              services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                   
                    options.ClientId = "783037398077-p9bttsb2cahm951sm805tdr0qbnl2k55.apps.googleusercontent.com";
                    options.ClientSecret = "ySRs4tkg9-O07EFKXUb0zPWZ";
                });
            
            services.AddControllersWithViews();

        }

        public void Configure(IApplicationBuilder app,IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
         
            app.UseRouting();
            
            app.UseIdentityServer();

             if(_env.IsDevelopment())
            {
                app.UseCookiePolicy(new CookiePolicyOptions()
                {
                     MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax
                });
            }

            // uncomment, if you want to add MVC
           // app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
