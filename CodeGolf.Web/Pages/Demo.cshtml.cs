using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Service;
using CodeGolf.Service.Dtos;
using CodeGolf.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;

namespace CodeGolf.Web.Pages
{
    [ServiceFilter(typeof(RecaptchaAttribute))]
    public class DemoModel : PageModel
    {
        private readonly ICodeGolfService codeGolfService;

        [BindProperty(BinderType = typeof(StringBinder))]
        public string Code { get; set; }

        public Option<int, ErrorSet> Result { get; private set; }

        public IChallengeSet ChallengeSet { get; }

        public DemoModel(ICodeGolfService codeGolfService)
        {
            this.codeGolfService = codeGolfService;
            this.ChallengeSet = codeGolfService.GetDemoChallenge();
        }

        public void OnGet()
        {

        }

        public async Task OnPost(CancellationToken cancellationToken)
        {
            if (this.ModelState.IsValid)
            {
                this.Result = await this.codeGolfService.Score(this.Code, this.ChallengeSet, cancellationToken).ConfigureAwait(false);
            }
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