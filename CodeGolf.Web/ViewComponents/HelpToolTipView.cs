using CodeGolf.Domain;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace CodeGolf.Web.ViewComponents
{
    public class HelpToolTipView : ViewComponent
    {
        public IViewComponentResult Invoke(Option<IChallengeSet> cs) =>
            cs.Match(this.View, () => (IViewComponentResult)this.Content(string.Empty));
    }
}