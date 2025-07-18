using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Enums;
using ParkNet.Entities.Identity;
using ParkNet.Entities.Infrastructure;
using ParkNet.Entities.Operations;


namespace ParkNet.Pages.Client.Subscriptions
{
    public class IndexModel : PageModel
    {
        private readonly ParkNetDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ParkNetDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<SelectListItem> SubscriptionTypeOptions { get; set; } = new();
        public List<SelectListItem> BuildingOptions { get; set; } = new();
        public List<SelectListItem> FloorOptions { get; set; } = new();
        public List<SelectListItem> ParkingSlotOptions { get; set; } = new();

        public decimal CalculatedPrice { get; set; }
        public List<Subscription> ActiveSubscriptions { get; set; } = new();

        public class InputModel
        {
            public SubscriptionType SubscriptionType { get; set; } = SubscriptionType.Mensal;
            public int BuildingId { get; set; }
            public int FloorId { get; set; }
            public int ParkingSlotId { get; set; }
            public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
        }

        public async Task OnGetAsync()
        {
            await LoadSelectListsAsync();
            CalculatedPrice = GetPrice(Input.SubscriptionType);
            await LoadActiveSubscriptionsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadSelectListsAsync();
            CalculatedPrice = GetPrice(Input.SubscriptionType);

            if (!ModelState.IsValid)
            {
                await LoadActiveSubscriptionsAsync();
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login");

            if (user.Balance < CalculatedPrice)
            {
                TempData["ErrorMessage"] = "Saldo insuficiente. Por favor, carregue saldo.";
                await LoadActiveSubscriptionsAsync();
                return Page();
            }

            bool slotReserved = await _context.Subscriptions
                .AnyAsync(s => s.ParkingSlotId == Input.ParkingSlotId && s.EndDate > DateTime.UtcNow);

            if (slotReserved)
            {
                TempData["ErrorMessage"] = "Este lugar já está reservado. Por favor, escolha outro.";
                await LoadActiveSubscriptionsAsync();
                return Page();
            }

            var endDate = GetEndDate(Input.StartDate, Input.SubscriptionType);

            var subscription = new Subscription
            {
                Type = Input.SubscriptionType,
                StartDate = Input.StartDate,
                EndDate = endDate,
                Price = CalculatedPrice,
                UserId = user.Id,
                ParkingSlotId = Input.ParkingSlotId
            };

            user.Balance -= CalculatedPrice;
            _context.Subscriptions.Add(subscription);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Avença adquirida com sucesso!";
            return RedirectToPage();
        }

        private decimal GetPrice(SubscriptionType type) => type switch
        {
            SubscriptionType.Mensal => 30,
            SubscriptionType.Trimestral => 80,
            SubscriptionType.Semestral => 150,
            SubscriptionType.Anual => 250,
            _ => 30
        };

        private DateTime GetEndDate(DateTime start, SubscriptionType type) => type switch
        {
            SubscriptionType.Mensal => start.AddMonths(1),
            SubscriptionType.Trimestral => start.AddMonths(3),
            SubscriptionType.Semestral => start.AddMonths(6),
            SubscriptionType.Anual => start.AddYears(1),
            _ => start.AddMonths(1)
        };

        private async Task LoadSelectListsAsync()
        {
            SubscriptionTypeOptions = Enum.GetValues(typeof(SubscriptionType))
                .Cast<SubscriptionType>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();

            BuildingOptions = await _context.Buildings
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToListAsync();

            if (Input.BuildingId > 0)
            {
                FloorOptions = await _context.Floors
                    .Where(f => f.BuildingId == Input.BuildingId)
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.Name
                    }).ToListAsync();
            }
            else
            {
                FloorOptions = new List<SelectListItem>();
            }

            if (Input.FloorId > 0)
            {
                ParkingSlotOptions = await _context.ParkingSlots
                    .Where(ps => ps.FloorId == Input.FloorId && !ps.Subscriptions.Any(s => s.EndDate > DateTime.UtcNow))
                    .Select(ps => new SelectListItem
                    {
                        Value = ps.Id.ToString(),
                        Text = ps.Spot
                    }).ToListAsync();
            }
            else
            {
                ParkingSlotOptions = new List<SelectListItem>();
            }
        }

        private async Task LoadActiveSubscriptionsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            ActiveSubscriptions = await _context.Subscriptions
                .Include(s => s.ParkingSlot)
                    .ThenInclude(ps => ps.Floor)
                        .ThenInclude(f => f.Building)
                .Where(s => s.UserId == user.Id && s.EndDate >
                DateTime.UtcNow)
                .ToListAsync();
        }
    }
}
