using ParkNet.Entities.Enums;
using ParkNet.Entities.Identity;

namespace ParkNet.Entities.Operations
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;

        // Relações
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}
