using System.Threading.Tasks;
using CodeGolf.Service;
using CodeGolf.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    [ValidateAntiForgeryToken]
    public class AdminModel : PageModel
    {
        private readonly IGameService gameService;

        public AdminModel(IGameService gameService)
        {
            this.gameService = gameService;
        }

        public async Task<IActionResult> OnPostResetGame()
        {
            await this.gameService.ResetGame();
            return this.RedirectToPage("Admin");
        }
    }
}