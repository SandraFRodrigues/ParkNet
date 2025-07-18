using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Identity;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ParkNet.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ParkNetDbContext _context;

        public DashboardModel(ParkNetDbContext context)
        {
            _context = context;
        }


        public int TotalUsers { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int OccupiedSlots { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalClientBalance { get; set; }
        public List<MonthlyRevenue> RevenuesByMonth { get; set; } = new();

        public class MonthlyRevenue
        {
            public string Month { get; set; }
            public decimal Revenue { get; set; }
        }

        public async Task OnGetAsync()
        {
            TotalUsers = await _context.Users.CountAsync();

            var today = DateTime.UtcNow;
            ActiveSubscriptions = await _context.Subscriptions.CountAsync(s => s.EndDate > today);

            OccupiedSlots = await _context.ParkingSlots.CountAsync(ps => ps.Status == ParkNet.Entities.Enums.SlotStatus.Ocupado);


            var subsRevenue = await _context.Subscriptions.SumAsync(s => (decimal?)s.Price) ?? 0;


            var parkRevenue = await _context.ParkingTransactions.Where(t => t.Paid).SumAsync(t => (decimal?)t.AmountCharged) ?? 0;

            TotalRevenue = subsRevenue + parkRevenue;


            TotalClientBalance = await _context.Users.SumAsync(u => (decimal?)u.Balance) ?? 0;


            for (int i = 0; i < 6; i++)
            {
                var month = today.AddMonths(-i);
                var subsMonth = await _context.Subscriptions
                    .Where(s => s.StartDate.Month == month.Month && s.StartDate.Year == month.Year)
                    .SumAsync(s => (decimal?)s.Price) ?? 0;

                var parkMonth = await _context.ParkingTransactions
                    .Where(t => t.EntryTime.Month == month.Month && t.EntryTime.Year == month.Year && t.Paid)
                    .SumAsync(t => (decimal?)t.AmountCharged) ?? 0;

                RevenuesByMonth.Add(new MonthlyRevenue
                {
                    Month = month.ToString("MM/yyyy"),
                    Revenue = subsMonth + parkMonth
                });
            }

            RevenuesByMonth = RevenuesByMonth.OrderBy(r => r.Month).ToList();
        }
    }
}

