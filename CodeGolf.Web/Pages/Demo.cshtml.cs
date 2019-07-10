using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;
using CodeGolf.Service;
using CodeGolf.Web.Attributes;
using CodeGolf.Web.Tooling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    using CodeGolf.Web.Controllers;
    using CodeGolf.Web.Models;

    using OneOf;
    using OneOf.Types;

    using Optional.Unsafe;

    [ValidateAntiForgeryToken]
    [ServiceFilter(typeof(RecaptchaAttribute))]
    public class DemoModel : PageModel
    {
        private readonly ICodeGolfService codeGolfService;

        [BindProperty(BinderType = typeof(StringBinder))]
        public string Code { get; set; }

        public OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet> Result { get; private set; }

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
                    a => a.Errors.Select(ErrorMessageParser.Parse).Where(x => x.HasValue).Select(
                        b =>
                            {
                                var z = b.ValueOrFailure();
                                return new ErrorItem(z.Line, z.Col);
                            }).ToArray());
                this.Result = r.Match(
                    a => (OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet>)a,
                    a => (OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet>)a.ToList(),
                    a => (OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet>)a);
            }
            else
            {
                var errors = this.ModelState.Values.SelectMany(a => a.Errors.Select(b => b.ErrorMessage)).ToList();
                this.Result =
                    (OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet>)new ErrorSet(errors);
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