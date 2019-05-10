using CodeGolf.Service.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.ViewComponents
{
    public class ChallengeView : ViewComponent
    {
        public IViewComponentResult Invoke(ChallengeSet<string> p) => this.View(p);
    }
}