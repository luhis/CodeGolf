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
using CodeGolf.Web.Attributes;
using CodeGolf.Web.Hubs;
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
using WebMarkupMin.AspNetCore2;

namespace CodeGolf.Web
{
    using CodeGolf.Web.HostedServices;
    using System.Threading.Tasks;

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
            services.AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = false;
                    options.AllowCompressionInDevelopmentEnvironment = false;
                })
                .AddHtmlMinification(
                    options =>
                    {
                        options.MinificationSettings.RemoveRedundantAttributes = true;
                        options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                        options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
                    })
                .AddHttpCompression();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie(configureOptions: cookieAuthenticationOptions => cookieAuthenticationOptions.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                })
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

            services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp/";
                });

            services.AddSignalR();
            services.AddHostedService<PingService>();
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IRunner runner, CodeGolfContext codeGolfContext)
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

            var minifiedGAScriptHash = "qEShyXJhwl9qHYlR7p7AwHlpnpEclnzFsPGivkRFOzM=";

            app.UseSecurityHeaders(policies =>
                    policies
                        .AddDefaultSecurityHeaders()
                        .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 63072000)
                        .AddContentSecurityPolicy(b =>
                        {
                            b.AddDefaultSrc().Self();
                            var scripts = b.AddScriptSrc().Self()
                                .From("https://www.google.com")
                                .From("https://www.googletagmanager.com")
                                .From("https://www.gstatic.com")
                                .From("https://www.google-analytics.com")
                                .WithHash256("fJYxG/MUxs9b4moaAfLG0e5TxMp0nppc6ulRT3MfHLU=").WithHash256(minifiedGAScriptHash);
                            if (env.IsDevelopment())
                            {
                                scripts.UnsafeEval();
                            }

                            b.AddImgSrc().Self().From("https://www.google-analytics.com").From("https://*.githubusercontent.com");
                            b.AddFrameSource().Self().From("https://www.google.com");
                            b.AddStyleSrc().Self().UnsafeInline().Blob();
                            b.AddConnectSrc().Self().From("https://localhost:8080");
                        }));

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
                        var cacheExtensions = new[] { ".css", ".js", ".gif", ".jpg", ".png", ".svg", ".ico" };
                        if (cacheExtensions.Any(path.EndsWith))
                        {
                            var maxAge = TimeSpan.FromDays(7);
                            r.Context.Response.Headers.Add("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
                        }
                    }
            });

            app.UseSpaStaticFiles();

            app.UseAuthentication();
            app.UseWebMarkupMin();

            app.UseCookiePolicy();
            app.UseSignalR(routes =>
            {
                routes.MapHub<RefreshHub>("/refreshHub");
            });
            app.UseMvc();

            app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp/codeGolf/";

                    if (env.IsDevelopment())
                    {
                        // run npm process with client app
                        //spa.UseReactDevelopmentServer(npmScript: "start:development");
                        // if you just prefer to proxy requests from client app, use proxy to SPA dev server instead:
                        // app should be already running before starting a .NET client
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:8080"); // your Vue app port
                    }
                });

            codeGolfContext.SeedDatabase().Wait();
            runner.WakeUpCompiler(CancellationToken.None);
        }
    }
}
