using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;

namespace CodeGolf.Web.Pages
{
    public class DashboardModel : PageModel
    {
        public Option<HoleDto> CurrentChallenge { get; private set; }

        public Option<Game> Game { get; private set; }

        private readonly IGameService gameService;

        public DashboardModel(IGameService gameService)
        {
            this.gameService = gameService;
        }

        public async Task OnGet()
        {
            this.Game = this.gameService.GetGame();
            var curr = await this.gameService.GetCurrentHole();
            this.CurrentChallenge = curr;
        }

        public async Task<IActionResult> OnPostNextHole()
        {
            await this.gameService.NextRound();
            return this.RedirectToPage("Dashboard");
        }
    }
}
