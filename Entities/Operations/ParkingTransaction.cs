using ParkNet.Entities.Identity;
using ParkNet.Entities.Infrastructure;

namespace ParkNet.Entities.Operations
{
    public class ParkingTransaction
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int ParkingSlotId { get; set; }
        public ParkingSlot? ParkingSlot { get; set; }

        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        public decimal? AmountCharged { get; set; }
        public bool Paid { get; set; } = false;
    }
}
