using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkNet.Entities.Infrastructure;
using ParkNet.Entities.Operations;
using ParkNet.Entities;

namespace ParkNet.Data
{
    public class ParkNetDbContext : IdentityDbContext<User>
    {
        public ParkNetDbContext(DbContextOptions<ParkNetDbContext> options)
            : base(options) { }

        // DbSets principais
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // BUILDING → FLOORS
            modelBuilder.Entity<Building>()
                .HasMany(b => b.Floors)
                .WithOne(f => f.Building)
                .HasForeignKey(f => f.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);

            // FLOOR → PARKING SLOTS
            modelBuilder.Entity<Floor>()
                .HasMany(f => f.ParkingSlots)
                .WithOne(ps => ps.Floor)
                .HasForeignKey(ps => ps.FloorId)
                .OnDelete(DeleteBehavior.Cascade);

            // PARKINGSLOT → VEHICLE TYPE
            modelBuilder.Entity<ParkingSlot>()
                .HasOne(ps => ps.VehicleType)
                .WithMany(vt => vt.ParkingSlots)
                .HasForeignKey(ps => ps.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // PARKINGSLOT → RESERVATIONS
            modelBuilder.Entity<ParkingSlot>()
                .HasMany(ps => ps.Reservations)
                .WithOne(r => r.ParkingSlot)
                .HasForeignKey(r => r.ParkingSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // PARKINGSLOT → SUBSCRIPTIONS
            modelBuilder.Entity<ParkingSlot>()
                .HasMany(ps => ps.Subscriptions)
                .WithOne(s => s.ParkingSlot)
                .HasForeignKey(s => s.ParkingSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // USER → RESERVATIONS
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // USER → SUBSCRIPTIONS
            modelBuilder.Entity<User>()
                .HasMany(u => u.Subscriptions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // USER → PAYMENT TRANSACTIONS
            modelBuilder.Entity<User>()
                .HasMany(u => u.PaymentTransactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ENUM → SUBSCRIPTION TYPE como string
            modelBuilder.Entity<Subscription>()
                .Property(s => s.Type)
                .HasConversion<string>();
        }
    }
}
