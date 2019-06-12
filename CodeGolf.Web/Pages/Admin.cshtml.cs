using System.Threading.Tasks;
using CodeGolf.Service;
using CodeGolf.Web.Attributes;
using CodeGolf.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    public class AdminModel : PageModel
    {
        private readonly IGameService gameService;
        private readonly ISignalRNotifier signalRNotifier;

        public AdminModel(IGameService gameService, ISignalRNotifier signalRNotifier)
        {
            this.gameService = gameService;
            this.signalRNotifier = signalRNotifier;
        }

        public async Task<IActionResult> OnPostResetGame()
        {
            await this.gameService.ResetGame();
            await this.signalRNotifier.NewRound();
            return this.RedirectToPage("Admin");
        }
    }
}