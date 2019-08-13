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
            while (!this.source.IsCancellationRequested)
            {
                await this.svc.Ping();
                await Task.Delay(TimeSpan.FromSeconds(30), this.source.Token);
            }
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            source.Cancel();
            return Task.CompletedTask;
        }
    }
}
