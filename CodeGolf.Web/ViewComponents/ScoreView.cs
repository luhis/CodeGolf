using System;
using System.Collections.Generic;

using CodeGolf.Domain.ChallengeInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.ViewComponents
{
    using CodeGolf.Service.Dtos;

    using OneOf;
    using OneOf.Types;

    public class ScoreView : ViewComponent
    {
        public IViewComponentResult Invoke(
            OneOf<None, int, IReadOnlyList<Domain.ChallengeResult>, IReadOnlyList<CompileErrorMessage>> p,
            IChallengeSet cs)
        {
            return p.Match(
                _ => this.Content(string.Empty),
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
