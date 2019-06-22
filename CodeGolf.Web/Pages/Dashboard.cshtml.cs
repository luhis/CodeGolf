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
    using Optional.Unsafe;

    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    [ValidateAntiForgeryToken]
    public class DashboardModel : PageModel
    {
        public Option<HoleDto> CurrentChallenge { get; private set; }

        private readonly IDashboardService gameService;

        public DashboardModel(IDashboardService gameService)
        {
            this.gameService = gameService;
        }

        public async Task OnGet(CancellationToken cancellationToken)
        {
            var curr = await this.gameService.GetCurrentHole(cancellationToken);
            this.CurrentChallenge = curr;
        }

        public async Task<IActionResult> OnPostEndHole(CancellationToken cancellationToken)
        {
            var curr = await this.gameService.GetCurrentHole(cancellationToken);
            await this.gameService.EndHole(curr.ValueOrFailure().Hole.HoleId);
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostNextHole()
        {
            var holeId = await this.gameService.NextHole();
            return holeId.Match(some => this.RedirectToPage(), () => this.RedirectToPage("Results"));
        }
    }
}
