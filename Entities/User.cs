using Microsoft.AspNetCore.Identity;

namespace ParkNet.Entities
{
    public class User : IdentityUser    
    {
        public ICollection<Reservation>? Reservations { get; set; } // //estender IdentityUser para futuras relações com reservations
    }
}
