using ParkNet.Entities.Infrastructure;
using System;

namespace ParkNet.Entities.Operations
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        public int ParkingSlotId { get; set; }
        public ParkingSlot? ParkingSlot { get; set; }
    }
}
