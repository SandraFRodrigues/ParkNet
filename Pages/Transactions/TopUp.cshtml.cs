using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkNet.Data;
using ParkNet.Entities.Operations;
using Microsoft.AspNetCore.Identity;
using ParkNet.Entities.Identity;
using ParkNet.Entities.Enums;

namespace ParkNet.Pages.Transactions
{
    public class TopUpModel : PageModel
    {
        private readonly ParkNetDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public decimal Amount { get; set; }

        public TopUpModel(ParkNetDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Amount <= 0)
            {
                ModelState.AddModelError(string.Empty, "O valor deve ser maior do que zero.");
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            user.Balance += Amount;

            var transaction = new PaymentTransaction
            {
                UserId = user.Id,
                Amount = Amount,
                Timestamp = DateTime.UtcNow,
                Description = $"Carregamento de saldo: {Amount:C}",
                Type = TransactionType.TopUp
            };
            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Saldo carregado com sucesso!";
            return RedirectToPage("/Index");
        }
    }
}
