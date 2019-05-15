using CodeGolf.Service.Dtos;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace CodeGolf.Web.ViewComponents
{
    public class ScoreView : ViewComponent
    {
        public IViewComponentResult Invoke(Option<int, ErrorSet> p)
        {
            return p.Match(score => this.View("Score", score), errors =>
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