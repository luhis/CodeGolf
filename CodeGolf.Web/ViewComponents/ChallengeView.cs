using CodeGolf.Domain;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace CodeGolf.Web.ViewComponents
{
    public class ChallengeView : ViewComponent
    {
        public IViewComponentResult Invoke(Option<ChallengeSet<string>> cs) =>
            cs.Match(this.View, () => this.View("None"));
    }
}