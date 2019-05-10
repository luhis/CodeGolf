using System.Collections.Generic;
using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    public class DashboardModel : PageModel
    {
        public ChallengeSet<string> CurrentChallenge { get; private set; }

        public IReadOnlyList<Attempt> Attempts { get; private set; }

        private readonly IGameService gameService;

        public DashboardModel(IGameService gameService)
        {
            this.gameService = gameService;
        }

        public void OnGet()
        {
            var curr = this.gameService.GetCurrent();
            this.CurrentChallenge = curr.ChallengeSet;
            this.Attempts = curr.Attempts;
        }
    }
}