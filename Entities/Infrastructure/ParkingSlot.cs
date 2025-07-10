using ParkNet.Entities.Enums;
using ParkNet.Entities.Operations;

namespace ParkNet.Entities.Infrastructure
{
    public class ParkingSlot
    {
        public int Id { get; set; }

        public int FloorId { get; set; }
        public Floor? Floor { get; set; }

        public int VehicleTypeId { get; set; }
        public VehicleType? VehicleType { get; set; }

        public int Order { get; set; } 

        public string Spot { get; set; } = string.Empty;

        public bool EmptySpot => string.IsNullOrWhiteSpace(Spot);

        public SlotStatus Status { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
