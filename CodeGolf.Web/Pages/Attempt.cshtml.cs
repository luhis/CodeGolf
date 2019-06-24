using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Service;
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

        public string Code { get; private set; }

        public async Task OnGet(Guid id, CancellationToken cancellationToken)
        {
            this.Code = (await this.dashboardService.GetAttemptById(id, cancellationToken)).Code;
        }
    }
}