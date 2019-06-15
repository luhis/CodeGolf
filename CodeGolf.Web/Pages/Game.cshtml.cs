using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Web.Hubs;
using CodeGolf.Web.Tooling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;
using Optional.Unsafe;

namespace CodeGolf.Web.Pages
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class GameModel : PageModel
    {
        private readonly IGameService gameService;
        private readonly IIdentityTools identityTools;

        [BindProperty(BinderType = typeof(StringBinder))]
        public string Code { get; set; }

        public Option<Option<int, IReadOnlyList<Domain.ChallengeResult>>, ErrorSet> Result { get; private set; }

        public Option<IChallengeSet> Hole { get; private set; }

        public GameModel(IGameService gameService, IIdentityTools identityTools)
        {
            this.gameService = gameService;
            this.identityTools = identityTools;
        }

        public async Task OnGet(CancellationToken cancellationToken)
        {
            this.Hole = (await this.gameService.GetCurrentHole(cancellationToken)).Map(a => a.Hole.ChallengeSet);
        }

        public async Task OnPost(CancellationToken cancellationToken)
        {
            var hole = await this.gameService.GetCurrentHole(cancellationToken);
            this.Hole = hole.Map(a => a.Hole.ChallengeSet);
            var gs = hole.ValueOrFailure();

            var res = 
                await this.gameService.Attempt(this.identityTools.GetIdentity(this.HttpContext).ValueOrFailure(), gs.Hole.HoleId, this.Code, gs.Hole.ChallengeSet, cancellationToken).ConfigureAwait(false);

            this.Result = res;
        }

        public IActionResult OnPostViewSource()
        {
            return this.RedirectToAction("Preview", "Code", new { this.Code });
        }

        public IActionResult OnPostViewDebugSource()
        {
            return this.RedirectToAction("Debug", "Code", new { this.Code });
        }
    }
}