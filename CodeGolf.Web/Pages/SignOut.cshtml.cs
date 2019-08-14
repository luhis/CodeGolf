namespace CodeGolf.Web.Pages
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Razor page")]
    public class SignOutModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<RedirectResult> OnPost()
        {
            await this.HttpContext.SignOutAsync();
            return this.Redirect(this.Url.Content("~/"));
        }
    }
}