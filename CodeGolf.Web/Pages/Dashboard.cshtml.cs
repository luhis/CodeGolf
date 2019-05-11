using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;

namespace CodeGolf.Web.Pages
{
    public class DashboardModel : PageModel
    {
        public Option<GameSlot> CurrentChallenge { get; private set; }

        public Option<Game> Game { get; private set; }

        private readonly IGameService gameService;

        public DashboardModel(IGameService gameService)
        {
            this.gameService = gameService;
        }

        public void OnGet()
        {
            this.Game = this.gameService.GetGame();
            var curr = this.gameService.GetCurrent();
            this.CurrentChallenge = curr;
        }
    }
}