namespace CodeGolf.Web.HostedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.ServiceInterfaces;
    using Microsoft.Extensions.Hosting;

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

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            this.source.Cancel();
            return Task.CompletedTask;
        }

        private async Task Loop()
        {
            while (!this.source.IsCancellationRequested)
            {
                await this.svc.Ping();
                await Task.Delay(TimeSpan.FromSeconds(30), this.source.Token);
            }
        }
    }
}
