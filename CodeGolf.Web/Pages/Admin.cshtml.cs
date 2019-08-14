namespace CodeGolf.Web.Pages
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain.ChallengeInterfaces;
    using CodeGolf.Service;
    using CodeGolf.Web.Attributes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [Authorize]
    [ServiceFilter(typeof(GameAdminAuthAttribute))]
    [ValidateAntiForgeryToken]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Razor Pages")]
    public class AdminModel : PageModel
    {
        private readonly IAdminService adminService;

        public AdminModel(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        public IReadOnlyList<IChallengeSet> Holes { get; private set; }

        public async Task OnGet(CancellationToken cancellationToken)
        {
            this.Holes = await this.adminService.GetAllHoles(cancellationToken);
        }

        public async Task<IActionResult> OnPostResetGame()
        {
            await this.adminService.ResetGame();
            return this.RedirectToPage();
        }
    }
}