using CodeGolf.Service.Dtos;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace CodeGolf.Web.ViewComponents
{
    public class TimesView : ViewComponent
    {
        public IViewComponentResult Invoke(Option<HoleDto> hole) =>
            hole.Match(this.View, () => (IViewComponentResult)this.Content(string.Empty));
    }
}