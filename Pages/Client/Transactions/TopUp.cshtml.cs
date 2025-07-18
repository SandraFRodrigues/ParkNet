using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkNet.Data;
using ParkNet.Entities.Identity;
using ParkNet.Entities.Operations;
using ParkNet.Entities.Enums;
using System;
using System.Threading.Tasks;

namespace ParkNet.Pages.Client.Transactions
{
    public class TopUpModel : PageModel
    {
        private readonly ParkNetDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TopUpModel(ParkNetDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public decimal Amount { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Amount <= 0)
            {
                TempData["ErrorMessage"] = "O valor deve ser maior do que zero.";
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login");

            user.Balance += Amount;

            _context.Users.Update(user);

            _context.PaymentTransactions.Add(new PaymentTransaction
            {
                UserId = user.Id,
                Amount = Amount,
                Timestamp = DateTime.UtcNow,
                Description = $"Carregamento de saldo: {Amount:C}",
                Type = TransactionType.TopUp
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Saldo carregado com sucesso!";
            return RedirectToPage("/Client/Transactions/TopUp");
        }
    }
}
