using CodeGolf.Service.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Web.ViewComponents
{
    public class ErrorsView : ViewComponent
    {
        public IViewComponentResult Invoke(ErrorSet p) => this.View(p);
    }
}