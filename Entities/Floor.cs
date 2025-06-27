namespace ParkNet.Entities
{
    public class Floor
    {
        public int Id { get; set; }
        public int Number { get; set; }

        public int BuildingId { get; set; }
        public Building? Building { get; set; } 

        public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>(); 
    }
}
