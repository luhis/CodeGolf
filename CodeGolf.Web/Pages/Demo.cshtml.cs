using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;

namespace CodeGolf.Web.Pages
{
    public class DemoModel : PageModel
    {
        private readonly ICodeGolfService codeGolfService;

        [BindProperty]
        public string Code { get; set; }

        public Option<int, ErrorSet> Result { get; private set; }

        public ChallengeSet<string> ChallengeSet { get; }

        public DemoModel(ICodeGolfService codeGolfService)
        {
            this.codeGolfService = codeGolfService;
            this.ChallengeSet = codeGolfService.GetDemoChallenge();
        }

        public void OnGet()
        {

        }

        public async Task OnPost()
        {
            this.Result = await this.codeGolfService.Score(this.Code, this.ChallengeSet).ConfigureAwait(false);
        }
    }
}