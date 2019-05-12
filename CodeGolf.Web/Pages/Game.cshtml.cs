using System.Threading.Tasks;
using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using CodeGolf.Web.Tooling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;
using Optional.Unsafe;

namespace CodeGolf.Web.Pages
{
    public class GameModel : PageModel
    {
        private readonly IGameService gameService;
        private readonly IIdentityTools identityTools;

        [BindProperty]
        public string Code { get; set; }

        public int? Score { get; private set; }

        public ErrorSet Errors { get; private set; } = new ErrorSet();

        public Option<Round> ChallengeSet { get; }

        public GameModel(IGameService gameService, IIdentityTools identityTools)
        {
            this.gameService = gameService;
            this.identityTools = identityTools;
            this.ChallengeSet = gameService.GetCurrent().Result; // fix this
        }

        public async Task OnPost()
        {
            var gs = this.ChallengeSet.ValueOrFailure();
            var res = await this.gameService.Attempt(this.identityTools.GetIdentity(this.Request), gs.RoundId, this.Code, gs.ChallengeSet).ConfigureAwait(false);
            res.Match(a => this.Score = a, err => this.Errors = err);
        }
    }
}