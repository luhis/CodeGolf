using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Web.Attributes;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    public class AttemptModel : PageModel
    {
        private readonly IDashboardService dashboardService;

        public AttemptModel(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        public AttemptCodeDto Code { get; private set; }

        public async Task<IActionResult> OnGet(Guid id, CancellationToken cancellationToken)
        {
            var res = await this.dashboardService.GetAttemptById(id, cancellationToken);
            return res.Match(
                some =>
                    {
                        this.Code = some;
                        return (IActionResult)this.Page();
                    },
                this.NotFound);
        }
    }
}