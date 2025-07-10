using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkNet.Services.Contracts;

namespace ParkNet.Pages.Admin.Import
{
    public class ViewImportModel : PageModel
    {
        private readonly IImportService _importService;

        public ViewImportModel(IImportService importService)
        {
            _importService = importService;
        }

        [BindProperty]
        public string LayoutText { get; set; }

        [BindProperty]
        public string BuildingName { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(LayoutText) || string.IsNullOrEmpty(BuildingName))
            {
                ModelState.AddModelError(string.Empty, "Todos os campos são obrigatórios.");
                return Page();
            }

            var success = await _importService.CreateBuildingFromLayoutAsync(BuildingName, LayoutText);

            if (success)
            {
                TempData["Success"] = "Parque importado com sucesso!";
                return RedirectToPage("/Buildings/Index");
            }

            ModelState.AddModelError(string.Empty, "Erro ao importar o parque.");
            return Page();
        }
    }
}












