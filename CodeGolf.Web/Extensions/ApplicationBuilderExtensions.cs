namespace CodeGolf.Web.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomSecurityHeaders(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            const string monacoHash = "VK+pKL3MRvBY2BmWZQQrf4dm90dj3F2xZ7/V3Zs07ko=";
            const string swaggerHash = "hQoyAYxxdlQX6mYg//3SgDUdhiDx4sZq5ThHlCL8Ssg=";
            const string swagger2Hash = "ip2mafwm8g4hzTmJd0ltVOzuizPeY1roJ3pkMwGXm8E=";
            const string styleBootstrap = "F1noxsLOnJhyRSgc0zu5JgzoLjG2BBMaXaSG24k2mRM=";
            const string googleCom = "https://www.google.com";
            const string googleAnal = "https://www.google-analytics.com";
            const string jsDelivr = "https://cdn.jsdelivr.net";
            return app.UseSecurityHeaders(
                policies => policies.AddDefaultSecurityHeaders()
                    .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 63072000)
                    .RemoveServerHeader()
                    .AddContentSecurityPolicy(
                        b =>
                        {
                            b.AddDefaultSrc().Self();
                            var scripts = b.AddScriptSrc().Self()
                                .From(googleCom)
                                .From("https://www.gstatic.com")
                                .From(googleAnal)
                                .From(jsDelivr)
                                .WithHash256(monacoHash)
                                .WithHash256(swaggerHash)
                                .WithHash256(swagger2Hash)
                                .WithHash256(styleBootstrap);
                            if (env.IsDevelopment())
                            {
                                scripts.UnsafeEval();
                            }

                            b.AddImgSrc().Self().From(googleAnal)
                                .From("https://*.githubusercontent.com").From("data:");
                            b.AddFrameSource().Self().From(googleCom);
                            b.AddStyleSrc().Self().UnsafeInline().Blob().From(jsDelivr);
                            var connect = b.AddConnectSrc().Self().From("https://github.com");
                            if (env.IsDevelopment())
                            {
                                connect.From("https://localhost:*");
                            }

                            b.AddWorkerSrc().Self().Data();
                            b.AddFontSrc().Self().From(jsDelivr);
                        }));
        }

        public static IApplicationBuilder UseSpaStaticFileCaching(this IApplicationBuilder app)
        {
            var sfOptions = new StaticFileOptions
            {
                OnPrepareResponse = r =>
                {
                    var path = r.File.PhysicalPath;
                    var cacheExtensions =
                        new[] { ".css", ".js", ".gif", ".jpg", ".png", ".svg", ".ico", ".json" };
                    if (cacheExtensions.Any(path.EndsWith))
                    {
                        var maxAge = TimeSpan.FromDays(7);
                        r.Context.Response.Headers.Add(
                            "Cache-Control",
                            "max-age=" + maxAge.TotalSeconds.ToString("0"));
                    }
                }
            };
            app.UseSpaStaticFiles(sfOptions);

            return app;
        }
    }
}
