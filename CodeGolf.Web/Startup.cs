using System;
using System.Collections.Generic;
using System.Linq;
using CodeGolf.Recaptcha;
using CodeGolf.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.Web
{
    public class Startup
    {
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

            services.Configure<RouteOptions>(options => {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });

            services.Configure<WebSiteSettings>(this.Configuration);
            services.Configure<RecaptchaSettings>(this.Configuration.GetSection("Recaptcha"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private static readonly IReadOnlyList<Action<IServiceCollection>> DiModules = new List<Action<IServiceCollection>>
        {
            CodeGolf.Web.DiModule.Add,
            CodeGolf.Service.DiModule.Add,
            CodeGolf.Recaptcha.DiModule.Add,
            CodeGolf.Persistence.DiModule.Add,
            CodeGolf.Persistence.Static.DiModule.Add,
        };

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSecurityHeaders(policies =>
                    policies
                        .AddDefaultSecurityHeaders()
                        .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 63072000)
                );

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse =
                    r =>
                    {
                        var path = r.File.PhysicalPath;
                        var cacheExtensions = new[] { ".css", ".js", ".gif", ".jpg", ".png", ".svg" };
                        if (cacheExtensions.Any(path.EndsWith))
                        {
                            var maxAge = TimeSpan.FromDays(7);
                            r.Context.Response.Headers.Add("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
                        }
                    }
            });
            app.UseCookiePolicy();

            app.UseMvc();

            app.ApplicationServices.GetService<IRunner>().WakeUpCompiler();
        }
    }
}
