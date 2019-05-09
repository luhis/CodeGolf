using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    public class DemoModel : PageModel
    {
        private readonly ICodeGolfService codeGolfService;

        [BindProperty]
        public string Code { get; set; }
        public int? Score { get; private set; }

        public ChallengeSet<string> ChallengeSet { get; private set; }

        public DemoModel(ICodeGolfService codeGolfService)
        {
            this.codeGolfService = codeGolfService;
            this.ChallengeSet = Challenges.HelloWorld;
        }

        public void OnGet()
        {

        }

        public async Task OnPost()
        {
            var res = await this.codeGolfService.Score(this.Code, this.ChallengeSet).ConfigureAwait(false);
            res.Match(a => this.Score = a, err =>
            {
                this.Score = null;
                this.Errors = err.Errors;
            });
        }

        public IReadOnlyList<string> Errors { get; private set; } = new List<string>();
    }
}