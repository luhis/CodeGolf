namespace CodeGolf.Web.HostedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.ServiceInterfaces;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class PingService : BackgroundService
    {
        private readonly ILogger<PingService> logger;
        private readonly IExecutionService svc;

        public PingService(IExecutionService svc, ILogger<PingService> logger)
        {
            this.svc = svc;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogDebug($"{nameof(PingService)} is starting.");

            stoppingToken.Register(() =>
                this.logger.LogDebug($" {nameof(PingService)} background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                this.logger.LogDebug($"{nameof(PingService)} task doing background work.");

                await this.svc.Ping();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            this.logger.LogDebug($"{nameof(PingService)} background task is stopping.");
        }
    }
}
