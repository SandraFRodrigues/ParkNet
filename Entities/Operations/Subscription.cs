using ParkNet.Entities.Infrastructure;
using ParkNet.Entities.Enums;
using ParkNet.Entities.Identity;

namespace ParkNet.Entities.Operations
{
    public class Subscription
    {
        public int Id { get; set; }
        public SubscriptionType Type { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }
        public bool IsActive=> DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

        //relações
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public int ParkingSlotId { get; set; }
        public ParkingSlot? ParkingSlot { get; set; }
    }
}
