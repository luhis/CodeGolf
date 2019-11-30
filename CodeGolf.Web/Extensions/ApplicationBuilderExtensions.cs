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
            const string monacoHash = "+w9x3gaUtwchFo1AI6q4N4R2TrkXYr7IsIlGpAPu3mQ=";
            const string swaggerHash = "o9YqryvYsqgDW0dwRml5lTp2xj7JFP318EeoJJNQS94=";
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
                                .From(jsDelivr).WithHash256(monacoHash).WithHash256(swaggerHash);
                            if (env.IsDevelopment())
                            {
                                scripts.UnsafeEval();
                            }

                            b.AddImgSrc().Self().From(googleAnal)
                                .From("https://*.githubusercontent.com").From("data:");
                            b.AddFrameSource().Self().From(googleCom);
                            b.AddStyleSrc().Self().UnsafeInline().Blob().From(jsDelivr);
                            b.AddConnectSrc().Self().From("https://localhost:*").From("https://github.com");
                            b.AddWorkerSrc().Self().Data();
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
