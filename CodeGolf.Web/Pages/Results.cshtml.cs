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

        private readonly IResultsService resultsService;

        public ResultsModel(IResultsService resultsService)
        {
            this.resultsService = resultsService;
        }

        public async Task OnGet(CancellationToken cancellationToken)
        {
            this.Results = await this.resultsService.GetFinalScores(cancellationToken);
        }
    }
}