using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Web.Tooling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;
using Optional.Unsafe;

namespace CodeGolf.Web.Pages
{
    using System.Linq;

    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Web.Models;

    using OneOf;
    using OneOf.Types;

    [Authorize]
    [ValidateAntiForgeryToken]
    public class GameModel : PageModel
    {
        private readonly IGameService gameService;
        private readonly IIdentityTools identityTools;

        [BindProperty(BinderType = typeof(StringBinder))]
        public string Code { get; set; }

        public OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet> Result { get; private set; }

        public Option<IChallengeSet> Hole { get; private set; }

        public ErrorItem[] CodeErrorLocations { get; private set; } = new ErrorItem[0];

        public GameModel(IGameService gameService, IIdentityTools identityTools)
        {
            this.gameService = gameService;
            this.identityTools = identityTools;
        }

        public async Task OnGet(CancellationToken cancellationToken)
        {
            this.Hole = (await this.gameService.GetCurrentHole(cancellationToken)).Map(a => a.ChallengeSet);
            this.Result = new None();
        }

        public async Task OnPost(CancellationToken cancellationToken)
        {
            var hole = await this.gameService.GetCurrentHole(cancellationToken);
            this.Hole = hole.Map(a => a.ChallengeSet);
            var gs = hole.ValueOrFailure();

            var res = await this.gameService.Attempt(
                          this.identityTools.GetIdentity(this.HttpContext).ValueOrFailure(),
                          gs.Hole.HoleId,
                          this.Code,
                          gs.ChallengeSet,
                          cancellationToken).ConfigureAwait(false);
            this.CodeErrorLocations = res.Match(
                _ => new ErrorItem[0],
                _ => new ErrorItem[0],
                a => a.Errors.Select(ErrorMessageParser.Parse).Where(x => x.HasValue).Select(
                    b =>
                        {
                            var z = b.ValueOrFailure();
                            return new ErrorItem(z.Line, z.Col);
                        }).ToArray());

            this.Result = res.Match(
                a => (OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet>)a,
                a => (OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet>)a.ToList(),
                a => (OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet>)a);
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