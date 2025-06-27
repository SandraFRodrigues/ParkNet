using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ParkNet.Entities;

namespace ParkNet.Data
{
    public class ParkNetDbContext : IdentityDbContext<User>
    {
        public ParkNetDbContext(DbContextOptions<ParkNetDbContext> options)
            : base(options) { }
        
        public DbSet<Reservation> Reservations { get; set; } 
        public DbSet<ParkingSlot> ParkingSlots { get; set; } 
        public DbSet<Floor> Floors { get; set; } 
        public DbSet<Building> Buildings { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
            modelBuilder.Entity<Building>()
                .HasMany(b => b.Floors)
                .WithOne(f => f.Building)
                .HasForeignKey(f => f.BuildingId);
            modelBuilder.Entity<Floor>()
                .HasMany(f => f.ParkingSlots)
                .WithOne(ps => ps.Floor)
                .HasForeignKey(ps => ps.FloorId);
            modelBuilder.Entity<ParkingSlot>()
                .HasMany(ps => ps.Reservations)
                .WithOne(r => r.ParkingSlot)
                .HasForeignKey(r => r.ParkingSlotId);
        }
    }
}
