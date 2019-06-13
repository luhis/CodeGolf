using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.ViewComponents
{
    public class AttemptsView : ViewComponent
    {
        public IViewComponentResult Invoke() => this.View();
    }
}