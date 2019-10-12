namespace CodeGolf.ExecutionServer
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.ServiceInterfaces;
    using JKang.IpcServiceFramework;
    using Microsoft.Extensions.DependencyInjection;

    public static class Program
    {
        public static async Task Main()
        {
            // configure DI
            var services = ConfigureServices(new ServiceCollection());

            // build and run service host
            var host = new IpcServiceHostBuilder(services.BuildServiceProvider())
                .AddTcpEndpoint<IExecutionService>(name: "endpoint2", ipEndpoint: IPAddress.Loopback, port: SharedSettings.PortNumber)
                .Build();
            Console.WriteLine("CodeGolf execution service");
            var source = new CancellationTokenSource();
            await Task.WhenAll(host.RunAsync(source.Token), Task.Run(() =>
            {
                Console.WriteLine("Press any key to shutdown.");
                Console.Read();
                source.Cancel();
            }));

            Console.WriteLine("Server stopped.");
        }

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services
                .AddIpc(builder => builder.AddService<IExecutionService, ExecutionService>());
        }
    }
}
