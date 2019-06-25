namespace CodeGolf.Web.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;

    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult SignIn(string returnUrl = "/")
        {
            return this.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }
    }
}