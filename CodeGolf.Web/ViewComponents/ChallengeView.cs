using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace CodeGolf.Web.ViewComponents
{
    public class ChallengeView : ViewComponent
    {
        public IViewComponentResult Invoke(Option<IChallengeSet> cs) =>
            cs.Match(this.View, () => this.View("None"));
    }
}