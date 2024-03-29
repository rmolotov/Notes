﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using System.Reflection;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using UserManagement.Core.Entities;
using UserManagement.Persistence.Database;

namespace Notes.Identity
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("UsersConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddDefaultTokenProviders();

            var builder = services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                    options.EmitStaticAudienceClaim = true;
                    options.IssuerUri = Configuration.GetValue<string>("IdentityUri");
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                            Configuration.GetConnectionString("IdentityServer"),
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            })
                        .ConfigureWarnings(warnings =>
                            warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                            Configuration.GetConnectionString("IdentityServer"),
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            })
                        .ConfigureWarnings(warnings =>
                            warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
                })
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app); // uncomment for add migrations and scopes/clients to ext db
            
            if (Environment.IsDevelopment())
            {
                Log.Information("is development");
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
        
        private void InitializeDatabase(IApplicationBuilder app)
        {
            Log.Information("Database initialization...");
            
            using var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>()
                .CreateScope();
            
            var persistedContext = serviceScope.ServiceProvider
                .GetRequiredService<PersistedGrantDbContext>();

            var configContext = serviceScope.ServiceProvider
                .GetRequiredService<ConfigurationDbContext>();

            try
            {
                persistedContext.Database.Migrate();
                configContext.Database.Migrate();
            }
            catch (SqlException exception) when (exception.ErrorCode == 1801)
            {
                Log.Warning("Database exists, skip initialization");
                throw;
            }

            if (!configContext.Clients.Any())
            {
                foreach (var client in Config.Clients) 
                    configContext.Clients.Add(client.ToEntity());
                    
                configContext.SaveChanges();
            }

            if (!configContext.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources) 
                    configContext.IdentityResources.Add(resource.ToEntity());
                    
                configContext.SaveChanges();
            }

            if (!configContext.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes) 
                    configContext.ApiScopes.Add(resource.ToEntity());
                    
                configContext.SaveChanges();
            }
        }
    }
}