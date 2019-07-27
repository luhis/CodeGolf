using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using CodeGolf.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    using Optional;

    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    [ValidateAntiForgeryToken]
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService gameService;

        public DashboardModel(IDashboardService gameService)
        {
            this.gameService = gameService;
        }

        public Option<HoleDto> CurrentChallenge { get; private set; }

        public async Task OnGet(CancellationToken cancellationToken)
        {
            var curr = await this.gameService.GetCurrentHole(cancellationToken);
            this.CurrentChallenge = curr;
        }

        public async Task<IActionResult> OnPostEndHole(CancellationToken cancellationToken)
        {
            var curr = await this.gameService.GetCurrentHole(cancellationToken);
            await curr.Match(
                some => this.gameService.EndHole(some.Hole.HoleId),
                () => Task.CompletedTask);
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostNextHole(CancellationToken cancellationToken)
        {
            var holeId = await this.gameService.NextHole(cancellationToken);
            return holeId.Match(some => this.RedirectToPage(), () => this.RedirectToPage("Results"));
        }
    }
}
