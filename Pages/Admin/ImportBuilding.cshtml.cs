using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Services.Contracts;
using System.Threading.Tasks;

namespace ParkNet.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ImportBuildingModel : PageModel
    {
        private readonly IBuildingImportService _importBuildingService;

        private readonly ParkNetDbContext _context;

        public ImportBuildingModel(IBuildingImportService importBuildingService, ParkNetDbContext context)

        {

            _importBuildingService = importBuildingService;

            _context = context;

        }


        [BindProperty]
        public string Layout { get; set; } = string.Empty;

        [BindProperty]
        public string BuildingName { get; set; } = string.Empty;

        public bool? Success { get; set; }

        public void OnGet()
        {
            Layout = string.Empty;
            BuildingName = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(BuildingName))
            {
                ModelState.AddModelError(nameof(BuildingName), "O nome do edifício é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(Layout))
            {
                ModelState.AddModelError(nameof(Layout), "O layout é obrigatório.");
            }

            if (!ModelState.IsValid)
            {
                Success = false;
                return Page();
            }

            Success = await _importBuildingService.ImportFromTextAsync(Layout, BuildingName);

            if (Success == true)
            {
                var lastBuilding = await _context.Buildings
                    .OrderByDescending(b => b.Id)
                    .FirstOrDefaultAsync();

                if (lastBuilding != null)
                {
                    return RedirectToPage("/Admin/Buildings/ViewBuilding", new { id = lastBuilding.Id });
                }
            }

            return Page();
        }
    }
}
