using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGolf.Web.Pages
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using CodeGolf.Service;
    using CodeGolf.Service.Dtos;
    using CodeGolf.Web.Attributes;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    public class ResultsModel : PageModel
    {
        public IReadOnlyList<ResultDto> Results { get; private set; }

        private readonly IDashboardService gameService;

        public ResultsModel(IDashboardService gameService)
        {
            this.gameService = gameService;
        }

        public async Task OnGet(CancellationToken cancellationToken)
        {
            this.Results = await this.gameService.GetFinalScores(cancellationToken);
        }
    }
}