using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CodeGolf.Web.Hubs
{
    public class SignalRNotifier : ISignalRNotifier
    {
        private readonly IHubContext<RefreshHub> hub;

        public SignalRNotifier(IHubContext<RefreshHub> hub)
        {
            this.hub = hub;
        }

        Task ISignalRNotifier.NewRound()
        {
            return this.hub.Clients.All.SendAsync("newRound");
        }

        public Task NewAnswer()
        {
            return this.hub.Clients.All.SendAsync("newAnswer");
        }
    }
}