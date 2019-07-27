using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain.ChallengeInterfaces;
using CodeGolf.Service;
using CodeGolf.Web.Attributes;
using CodeGolf.Web.Tooling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    using CodeGolf.Service.Dtos;
    using CodeGolf.Web.Models;

    using OneOf;
    using OneOf.Types;

    [ValidateAntiForgeryToken]
    [ServiceFilter(typeof(RecaptchaAttribute))]
    public class DemoModel : PageModel
    {
        private readonly ICodeGolfService codeGolfService;

        [BindProperty(BinderType = typeof(StringBinder))]
        public string Code { get; set; }

        public OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, IReadOnlyList<CompileErrorMessage>> Result { get; private set; }

        public IChallengeSet ChallengeSet { get; }

        public ErrorItem[] CodeErrorLocations { get; private set; } = new ErrorItem[0];

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
                var r = await this.codeGolfService.Score(this.Code, this.ChallengeSet, cancellationToken)
                            .ConfigureAwait(false);
                this.CodeErrorLocations = r.Match(
                    _ => new ErrorItem[0],
                    _ => new ErrorItem[0],
                    a => a.Select(
                        b => new ErrorItem(b.Line, b.Col, b.EndCol)).ToArray());
                this.Result = r.Match<OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, IReadOnlyList<CompileErrorMessage>>>(
                    a => a,
                    a => a.ToArray(),
                    a => a.ToArray());
            }
            else
            {
                var errors = this.ModelState.Values.SelectMany(a => a.Errors.Select(b => b.ErrorMessage));
                this.Result = errors.Select(a => new CompileErrorMessage(a)).ToArray();
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