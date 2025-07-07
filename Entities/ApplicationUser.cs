using Microsoft.AspNetCore.Identity;
using ParkNet.Entities.Operations;
using System.Collections.Generic;


namespace ParkNet.Entities
{
    public class User : IdentityUser    
    {
        public decimal Balance { get; set; } = 0;
        public string PaymentCardToken { get; set; }= string.Empty;
        public bool IsDriverLicenseValidated { get; set; } 
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>(); 
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>(); 
    }
}
