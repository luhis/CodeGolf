using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.ViewComponents
{
    public class CodeInputView : ViewComponent
    {
        public IViewComponentResult Invoke() => this.View();
    }
}