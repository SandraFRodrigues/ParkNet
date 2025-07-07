using ParkNet.Entities.Infrastructure;

public class Floor
{
    public int Id { get; set; }

    public int BuildingId { get; set; }
    public Building Building { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
}
