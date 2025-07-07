using System.Collections.Generic;

namespace ParkNet.Entities.Infrastructure
{
    public class Building
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<Floor> Floors { get; set; } = new List<Floor>();
        public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
    }
}
