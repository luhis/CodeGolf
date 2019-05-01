using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    public class DemoModel : PageModel
    {
        private readonly ICodeGolfService codeGolfService;

        [BindProperty]
        public string Code { get; set; }
        public int Score { get; private set; }

        public DemoModel(ICodeGolfService codeGolfService)
        {
            this.codeGolfService = codeGolfService;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            var challenge = Challenges.HelloWorld;
            var res = this.codeGolfService.Score(this.Code, challenge);
            res.Match(a => this.Score = a, err => this.Errors = err.Errors);
        }

        public IReadOnlyList<string> Errors { get; private set; } = new List<string>();
    }
}