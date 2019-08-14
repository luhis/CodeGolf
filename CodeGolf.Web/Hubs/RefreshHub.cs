namespace CodeGolf.Web.Hubs
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class RefreshHub : Hub
    {
    }
}