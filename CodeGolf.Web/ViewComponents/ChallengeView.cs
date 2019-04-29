using CodeGolf.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.ViewComponents
{
    public class ChallengeView : ViewComponent
    {
        public IViewComponentResult Invoke(Challenge<string> p) => this.View(p);
    }
}