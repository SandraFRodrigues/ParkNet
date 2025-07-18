using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkNet.Data;
using ParkNet.Entities.Identity;
using ParkNet.Entities.Infrastructure;
using ParkNet.Entities.Operations;
using ParkNet.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkNet.Pages.Client.Parking
{
    public class DashboardModel : PageModel
    {
        private readonly ParkNetDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardModel(ParkNetDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public EntryInputModel Input { get; set; } = new EntryInputModel();

        public List<SelectListItem> BuildingOptions { get; set; } = new();
        public List<SelectListItem> FloorOptions { get; set; } = new();
        public List<SelectListItem> SlotOptions { get; set; } = new();

        public decimal? UserBalance { get; set; }
        public bool HasActiveParking { get; set; }
        public ActiveEntryInfo? ActiveEntry { get; set; }

        public class EntryInputModel
        {
            public int BuildingId { get; set; }
            public int FloorId { get; set; }
            public int ParkingSlotId { get; set; }
        }

        public class ActiveEntryInfo
        {
            public int TransactionId { get; set; }
            public string Spot { get; set; } = "";
            public string FloorName { get; set; } = "";
            public string BuildingName { get; set; } = "";
            public DateTime EntryTime { get; set; }
            public string VehicleType { get; set; } = "";
        }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDataAsync();

            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login");

            // Verifica se já está estacionado
            bool jaEstacionado = await _context.ParkingTransactions
                .AnyAsync(t => t.UserId == user.Id && t.ExitTime == null);

            if (jaEstacionado)
            {
                TempData["ErrorMessage"] = "Já tem um veículo estacionado. Saia antes de entrar novamente!";
                return RedirectToPage();
            }

            var slot = await _context.ParkingSlots
                .Include(s => s.VehicleType)
                .FirstOrDefaultAsync(s => s.Id == Input.ParkingSlotId && s.Status == SlotStatus.Livre);

            if (slot == null)
            {
                TempData["ErrorMessage"] = "Lugar de estacionamento indisponível.";
                return RedirectToPage();
            }

            slot.Status = SlotStatus.Ocupado;

            _context.ParkingTransactions.Add(new ParkingTransaction
            {
                UserId = user.Id,
                ParkingSlotId = slot.Id,
                EntryTime = DateTime.UtcNow,
                AmountCharged = 0,
                Paid = false
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Entrada registada! O lugar {slot.Spot} ({slot.VehicleType?.Designation}) está reservado para si.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExitAsync(int transactionId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var transaction = await _context.ParkingTransactions
                .Include(t => t.ParkingSlot)
                .ThenInclude(ps => ps.VehicleType)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == user.Id && t.ExitTime == null);

            if (transaction == null)
            {
                TempData["ErrorMessage"] = "Não foi encontrada uma entrada ativa.";
                return RedirectToPage();
            }

            // 1. Calcular valor a cobrar
            var tempoMinutos = (DateTime.UtcNow - transaction.EntryTime).TotalMinutes;
            decimal tarifaPorMinuto = 0.10m; // 0.10€/minuto, podes ajustar
            var valorCobrado = Math.Round((decimal)tempoMinutos * tarifaPorMinuto, 2);

            // 2. Verifica saldo suficiente
            if (user.Balance < valorCobrado)
            {
                TempData["ErrorMessage"] = $"Saldo insuficiente para pagamento ({valorCobrado:F2}€ necessários).";
                return RedirectToPage();
            }

            // 3. Debitar saldo
            user.Balance -= valorCobrado;

            // 4. Atualiza transação e lugar
            transaction.AmountCharged = valorCobrado;
            transaction.ExitTime = DateTime.UtcNow;
            transaction.Paid = true;
            transaction.ParkingSlot.Status = SlotStatus.Livre;

            // 5. Regista pagamento
            _context.PaymentTransactions.Add(new PaymentTransaction
            {
                UserId = user.Id,
                Amount = valorCobrado,
                Timestamp = DateTime.UtcNow,
                Description = $"Pagamento estacionamento ({tempoMinutos:F0} min)"
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Saída registada com sucesso! Valor cobrado: €{valorCobrado:F2}";
            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            UserBalance = user?.Balance ?? 0;

            BuildingOptions = await _context.Buildings
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                .ToListAsync();

            FloorOptions = Input.BuildingId > 0
                ? await _context.Floors
                    .Where(f => f.BuildingId == Input.BuildingId)
                    .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Name })
                    .ToListAsync()
                : new List<SelectListItem>();

            SlotOptions = Input.FloorId > 0
                ? await _context.ParkingSlots
                    .Include(s => s.VehicleType)
                    .Where(s => s.FloorId == Input.FloorId && s.Status == SlotStatus.Livre)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Spot + " (" + (s.VehicleType != null ? s.VehicleType.Designation : "—") + ")"
                    })
                    .ToListAsync()
                : new List<SelectListItem>();

            var entry = await _context.ParkingTransactions
                .Where(t => t.UserId == user.Id && t.ExitTime == null)
                .Include(t => t.ParkingSlot).ThenInclude(ps => ps.Floor).ThenInclude(f => f.Building)
                .Include(t => t.ParkingSlot).ThenInclude(ps => ps.VehicleType)
                .FirstOrDefaultAsync();

            HasActiveParking = entry != null;
            if (HasActiveParking && entry != null)
            {
                ActiveEntry = new ActiveEntryInfo
                {
                    TransactionId = entry.Id,
                    Spot = entry.ParkingSlot.Spot,
                    FloorName = entry.ParkingSlot.Floor.Name,
                    BuildingName = entry.ParkingSlot.Floor.Building.Name,
                    EntryTime = entry.EntryTime,
                    VehicleType = entry.ParkingSlot.VehicleType?.Designation ?? ""
                };
            }
        }
    }
}

