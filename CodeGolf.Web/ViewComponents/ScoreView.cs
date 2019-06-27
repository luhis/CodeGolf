using System;
using System.Collections.Generic;
using CodeGolf.Domain;
using CodeGolf.Domain.ChallengeInterfaces;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace CodeGolf.Web.ViewComponents
{
    public class ScoreView : ViewComponent
    {
        public IViewComponentResult Invoke(Option<Option<int, IReadOnlyList<Domain.ChallengeResult>>, ErrorSet> p,
            IChallengeSet cs)
        {
            return p.Match(
                score => score.Match(num => this.View("Score", num),
                    err => this.View("ChallengeErrors", ValueTuple.Create(err, cs))), errors =>
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
