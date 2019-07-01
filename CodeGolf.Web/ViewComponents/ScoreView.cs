using System;
using System.Collections.Generic;
using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.ViewComponents
{
    using OneOf;

    public class ScoreView : ViewComponent
    {
        public IViewComponentResult Invoke(
            OneOf<int, IReadOnlyList<Domain.ChallengeResult>, ErrorSet> p,
            IChallengeSet cs)
        {
            return p.Match(
                num => this.View("Score", num),
                err => this.View("ChallengeErrors", ValueTuple.Create(err, cs)),
                errors =>
                    {
                        if (errors == null)
                        {
                            return (IViewComponentResult)this.Content(string.Empty);
                        }
                        else
                        {
                            return this.View("Errors", errors);
                        }
                    });
        }
    }
}
