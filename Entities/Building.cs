namespace ParkNet.Entities
{
    public class Building
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 

        public ICollection<Floor> Floors { get; set; } = new List<Floor>(); 
    }
}
