using ParkNet.Entities.Infrastructure;

namespace ParkNet.Entities.Enums
{
    public class VehicleType
    {
        public int Id { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
    }
}
