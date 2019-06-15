using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CodeGolf.Web.Hubs
{
    [Authorize]
    public class RefreshHub : Hub
    {
    }
}