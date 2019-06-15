using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using CodeGolf.Web.Attributes;
using CodeGolf.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;

namespace CodeGolf.Web.Pages
{
    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    [ValidateAntiForgeryToken]
    public class DashboardModel : PageModel
    {
        public Option<HoleDto> CurrentChallenge { get; private set; }

        private readonly IGameService gameService;

        public DashboardModel(IGameService gameService)
        {
            this.gameService = gameService;
        }

        public async Task OnGet(CancellationToken cancellationToken)
        {
            var curr = await this.gameService.GetCurrentHole(cancellationToken);
            this.CurrentChallenge = curr;
        }

        public async Task<IActionResult> OnPostNextHole()
        {
            await this.gameService.NextRound();
            return this.RedirectToPage("Dashboard");
        }
    }
}
