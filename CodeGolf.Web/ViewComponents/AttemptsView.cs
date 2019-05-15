using System.Collections.Generic;
using CodeGolf.Domain;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace CodeGolf.Web.ViewComponents
{
    public class AttemptsView : ViewComponent
    {
        public IViewComponentResult Invoke(Option<IReadOnlyList<Attempt>> cs) =>
            cs.Match(this.View, () => (IViewComponentResult)this.Content(string.Empty));
    }
}