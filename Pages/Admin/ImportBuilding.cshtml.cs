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
        private readonly IImportService _importService;
        private readonly ParkNetDbContext _context;

        public ImportBuildingModel(IImportService importService, ParkNetDbContext context)
        {
            _importService = importService;
            _context = context;
        }

        [BindProperty]
        public string Layout { get; set; }

        [BindProperty]
        public string BuildingName { get; set; }

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
                ModelState.AddModelError("BuildingName", "O nome do edifício é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(Layout))
            {
                ModelState.AddModelError("Layout", "O layout é obrigatório.");
            }

            if (!ModelState.IsValid)
            {
                Success = false;
                return Page();
            }

            Success = await _importService.CreateBuildingFromLayoutAsync(BuildingName, Layout);

            if (Success == true)
            {
                var lastBuilding = await _context.Buildings
                    .OrderByDescending(b => b.Id)
                    .FirstOrDefaultAsync();

                if (lastBuilding != null)
                {
                    return RedirectToPage("/Admin/ViewBuilding", new { id = lastBuilding.Id });
                }
            }

            return Page();
        }
    }
}
