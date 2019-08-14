namespace CodeGolf.Web.Hubs
{
    using System.Threading.Tasks;
    using CodeGolf.Service;
    using Microsoft.AspNetCore.SignalR;

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

        Task ISignalRNotifier.NewAnswer()
        {
            return this.hub.Clients.All.SendAsync("newAnswer");
        }

        Task ISignalRNotifier.NewTopScore(string userName, int score, string avatarUri)
        {
            return this.hub.Clients.All.SendAsync("newTopScore", userName, score, avatarUri);
        }
    }
}