namespace CodeGolf.Web
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public static class Program
    {
        public static Task Main(string[] args)
        {
            return CreateWebHostBuilder(args).Build().RunAsync();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
