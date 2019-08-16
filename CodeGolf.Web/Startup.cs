namespace CodeGolf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using CodeGolf.Persistence;
    using CodeGolf.Recaptcha;
    using CodeGolf.Service;
    using CodeGolf.Web.Extensions;
    using CodeGolf.Web.HostedServices;
    using CodeGolf.Web.Hubs;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using WebMarkupMin.AspNetCore3;

    public class Startup
    {
        private static readonly IReadOnlyList<Action<IServiceCollection>> DiModules = new List<Action<IServiceCollection>>
        {
            DiModule.Add,
            Service.DiModule.Add,
            CodeGolf.Recaptcha.DiModule.Add,
            Persistence.DiModule.Add,
            Persistence.Static.DiModule.Add,
            CollectableAssembly.DiModule.Add,
        };

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var module in DiModules)
            {
                module(services);
            }

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });

            static void BindNonPublic(BinderOptions o) => o.BindNonPublicProperties = true;

            services.Configure<GameAdminSettings>(this.Configuration, BindNonPublic);
            services.Configure<RecaptchaSettings>(this.Configuration.GetSection("Recaptcha"), BindNonPublic);
            services.Configure<ExecutionSettings>(this.Configuration.GetSection("Execution"), BindNonPublic);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddCustomResponseCompression();

            services.AddGitHubAuth(this.Configuration);

            services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp/";
                });

            services.AddSignalR();
            services.AddMediatR(typeof(GameService).Assembly);
            services.AddHostedService<PingService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Code Golf API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRunner runner, CodeGolfContext codeGolfContext)
        {
            ////if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ////else
            ////{
            ////    app.UseExceptionHandler("/Error");

            ////    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            ////    app.UseHsts();
            ////}

            app.UseCustomSecurityHeaders(env);

            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                    {
                        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                    });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFileCaching();

            app.UseRouting();
            app.UseAuthentication().UseAuthorization();
            app.UseWebMarkupMin();

            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RefreshHub>("/refreshHub");
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseSpa(
                spa =>
                    {
                        spa.Options.SourcePath = "ClientApp";

                        if (env.IsDevelopment())
                        {
                            // run npm process with client app
                            spa.UseReactDevelopmentServer(npmScript: "dev");

                            // if you just prefer to proxy requests from client app, use proxy to SPA dev server instead:
                            // app should be already running before starting a .NET client
                            // spa.UseProxyToSpaDevelopmentServer("http://localhost:8080"); // your Vue app port
                        }
                    });
            codeGolfContext.SeedDatabase();
            runner.WakeUpCompiler(CancellationToken.None);
        }
    }
}
