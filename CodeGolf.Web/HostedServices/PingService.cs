using System;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.ServiceInterfaces;
using Microsoft.Extensions.Hosting;

namespace CodeGolf.Web.HostedServices
{
    public class PingService : IHostedService
    {
        private readonly IExecutionService svc;
        private readonly CancellationTokenSource source;

        public PingService(IExecutionService svc)
        {
            this.svc = svc;
            this.source = new CancellationTokenSource();
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            return this.Loop();
        }

        private async Task Loop()
        {
            await this.svc.Ping();
            while (!this.source.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), this.source.Token);
                await this.svc.Ping();
            }
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            source.Cancel();
            return Task.CompletedTask;
        }
    }
}
