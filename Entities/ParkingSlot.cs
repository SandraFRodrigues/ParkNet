namespace ParkNet.Entities
{
    public class ParkingSlot
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; 
        public bool IsAvailable { get; set; } = true;

        public int FloorId { get; set; }
        public Floor? Floor { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>(); //Inicializar a lista para evitar nulos
    }
}
