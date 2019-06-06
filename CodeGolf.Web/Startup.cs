using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using CodeGolf.Persistence;
using CodeGolf.Recaptcha;
using CodeGolf.Service;
using CodeGolf.ServiceInterfaces;
using CodeGolf.Web.Attributes;
using JKang.IpcServiceFramework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

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

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });

            services.Configure<WebSiteSettings>(this.Configuration);
            services.Configure<GameAdminSettings>(this.Configuration);
            services.Configure<RecaptchaSettings>(this.Configuration.GetSection("Recaptcha"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie()
            .AddOAuth("GitHub", options =>
            {
                options.ClientId = this.Configuration["GitHub:ClientId"];
                options.ClientSecret = this.Configuration["GitHub:ClientSecret"];
                options.CallbackPath = new PathString("/signin-github");

                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey("urn:github:login", "login");
                options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        using (var request =
                            new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint))
                        {
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            using (var response = await context.Backchannel.SendAsync(request,
                                HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted))
                            {
                                response.EnsureSuccessStatusCode();

                                var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                                context.RunClaimActions(user);
                            }
                        }
                    }
                };
            });
        }

        private static readonly IReadOnlyList<Action<IServiceCollection>> DiModules = new List<Action<IServiceCollection>>
        {
            DiModule.Add,
            Service.DiModule.Add,
            CodeGolf.Recaptcha.DiModule.Add,
            Persistence.DiModule.Add,
            Persistence.Static.DiModule.Add,
        };

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CodeGolfContext codeGolfContext, IExecutionService svc)
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
                        .AddContentSecurityPolicy(b =>
                        {
                            b.AddDefaultSrc().Self();
                            b.AddScriptSrc().Self()
                                .From("https://www.google.com")
                                .From("https://www.googletagmanager.com")
                                .From("https://www.gstatic.com")
                                .From("https://www.google-analytics.com")
                                .WithHash256("fJYxG/MUxs9b4moaAfLG0e5TxMp0nppc6ulRT3MfHLU=");
                            b.AddImgSrc().Self().From("https://www.google-analytics.com");
                            b.AddFrameSource().Self().From("https://www.google.com");
                            b.AddStyleSrc().Self().UnsafeInline();
                        })
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

            app.UseAuthentication();

            app.UseCookiePolicy();

            app.UseMvc();

            codeGolfContext.SeedDatabase().Wait();
            app.ApplicationServices.GetService<IRunner>().WakeUpCompiler(CancellationToken.None);

            if (!svc.IsAlive().Result)
            {
                throw new Exception("Cannot connect to service");
            }
        }
    }
}
