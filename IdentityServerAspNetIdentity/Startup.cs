﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CompanyEmployees.IDP.Entities;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace CompanyEmployees.IDP
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; set; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<UserContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("identitySqlConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UserContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = c =>
                    c.UseSqlServer(Configuration.GetConnectionString("sqlConnection"),
                    sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = c =>
                    c.UseSqlServer(Configuration.GetConnectionString("sqlConnection"),
                    sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddAspNetIdentity<User>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
