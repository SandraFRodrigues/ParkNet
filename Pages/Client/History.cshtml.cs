using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkNet.Pages.Client
{
    public class HistoryModel : PageModel
    {
        private readonly ParkNetDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HistoryModel(ParkNetDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class Movimento
        {
            public string Tipo { get; set; } = "";
            public string Descricao { get; set; } = "";
            public decimal Valor { get; set; }
            public DateTime DataHora { get; set; }
        }

        public List<Movimento> Movimentos { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            var transacoes = await _context.PaymentTransactions
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.Timestamp)
                .Select(t => new Movimento
                {
                    Tipo = t.Type.ToString(),
                    Descricao = t.Description,
                    Valor = t.Amount,
                    DataHora = t.Timestamp
                })
                .ToListAsync();

            var parqueios = await _context.ParkingTransactions
                .Where(t => t.UserId == user.Id && t.ExitTime != null)
                .OrderByDescending(t => t.ExitTime)
                .Select(t => new Movimento
                {
                    Tipo = "Estacionamento",
                    Descricao = "Estacionamento em " + (t.ParkingSlot != null ? t.ParkingSlot.Spot : ""),
                    Valor = t.AmountCharged ?? 0,
                    DataHora = t.ExitTime ?? DateTime.UtcNow
                })
                .ToListAsync();

            Movimentos = transacoes.Concat(parqueios).OrderByDescending(m => m.DataHora).ToList();
        }
    }
}
