using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using CodeGolf.Web.Tooling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;
using Optional.Unsafe;

namespace CodeGolf.Web.Pages
{
    [Authorize]
    public class GameModel : PageModel
    {
        private readonly IGameService gameService;
        private readonly IIdentityTools identityTools;

        [BindProperty(BinderType = typeof(StringBinder))]
        public string Code { get; set; }

        public Option<int, ErrorSet> Result { get; private set; }

        public Option<ChallengeSet<string>> Round { get; private set; }

        public GameModel(IGameService gameService, IIdentityTools identityTools)
        {
            this.gameService = gameService;
            this.identityTools = identityTools;
        }

        public async Task OnGet()
        {
            this.Round = (await this.gameService.GetCurrentHole()).Map(a => a.Hole.ChallengeSet);
        }

        public async Task OnPost(CancellationToken cancellationToken)
        {
            var round = await this.gameService.GetCurrentHole();
            this.Round = round.Map(a => a.Hole.ChallengeSet);
            var gs = round.ValueOrFailure();

            this.Result = 
                await this.gameService.Attempt(this.identityTools.GetIdentity(this.HttpContext).ValueOrFailure(), gs.Hole.HoleId, this.Code, gs.Hole.ChallengeSet, cancellationToken).ConfigureAwait(false);
        }

        public IActionResult OnPostViewSource()
        {
            return this.RedirectToAction("Preview", "Code", new { this.Code });
        }
    }
}