using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkNet.Entities.Enums;
using ParkNet.Entities.Identity;
using ParkNet.Entities.Infrastructure;
using ParkNet.Entities.Operations;

namespace ParkNet.Data
{
    public class ParkNetDbContext : IdentityDbContext<ApplicationUser>
    {
        public ParkNetDbContext(DbContextOptions<ParkNetDbContext> options)
            : base(options) { }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<ParkingTransaction> ParkingTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
            modelBuilder.Entity<Building>()
                .HasMany(b => b.Floors)
                .WithOne(f => f.Building)
                .HasForeignKey(f => f.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
        
            modelBuilder.Entity<Floor>()
                .HasMany(f => f.ParkingSlots)
                .WithOne(ps => ps.Floor)
                .HasForeignKey(ps => ps.FloorId)
                .OnDelete(DeleteBehavior.Cascade);
        
            modelBuilder.Entity<ParkingSlot>()
                .HasOne(ps => ps.VehicleType)
                .WithMany(vt => vt.ParkingSlots)
                .HasForeignKey(ps => ps.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);
          
            modelBuilder.Entity<ParkingSlot>()
                .HasMany(ps => ps.Reservations)
                .WithOne(r => r.ParkingSlot)
                .HasForeignKey(r => r.ParkingSlotId)
                .OnDelete(DeleteBehavior.Restrict);
        
            modelBuilder.Entity<ParkingSlot>()
                .HasMany(ps => ps.Subscriptions)
                .WithOne(s => s.ParkingSlot)
                .HasForeignKey(s => s.ParkingSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Subscriptions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.PaymentTransactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Subscription>()
                .Property(s => s.Type)
                .HasConversion<string>();

            modelBuilder.Entity<ParkingSlot>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.Timestamp)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<PaymentTransaction>()
                .HasIndex(p => p.UserId);

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.UserId);
        }
    }
}
