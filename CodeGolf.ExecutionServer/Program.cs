using System;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.ServiceInterfaces;
using JKang.IpcServiceFramework;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGolf.ExecutionServer
{
    public static class Program
    {
        static async Task Main()
        {
            // configure DI
            var services = ConfigureServices(new ServiceCollection());

            // build and run service host
            var host = new IpcServiceHostBuilder(services.BuildServiceProvider())
                .AddNamedPipeEndpoint<IExecutionService>(name: "endpoint2", pipeName: SharedSettings.PipeName)
                .Build();

            var source = new CancellationTokenSource();
            await Task.WhenAll(host.RunAsync(source.Token), Task.Run(() =>
            {
                Console.WriteLine("Press any key to shutdown.");
                Console.ReadKey();
                source.Cancel();
            }));

            Console.WriteLine("Server stopped.");
        }

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services
                .AddIpc(builder =>
                {
                    builder
                        .AddNamedPipe(options =>
                        {
                            options.ThreadCount = 2;
                        })
                        .AddService<IExecutionService, ExecutionService>();
                });
        }
    }
}
