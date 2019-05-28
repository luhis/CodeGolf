using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
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