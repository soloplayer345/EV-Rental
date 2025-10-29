using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public partial class EVRentalDBContext : DbContext
    {
        public EVRentalDBContext()
        {
        }

        public EVRentalDBContext(DbContextOptions<EVRentalDBContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<RentalRecord> RentalRecords { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<InspectionProblem> InspectionProblems { get; set; }
        public DbSet<RatingReview> RatingReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Account
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.PasswordHash).HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(false);
            });

            // Configure Station
            modelBuilder.Entity<Station>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            });

            // Configure Vehicle
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Brand).HasMaxLength(50);
                entity.Property(e => e.PlateNumber).HasMaxLength(50);
                entity.Property(e => e.Model).HasMaxLength(100);
                entity.Property(e => e.VehicleType).HasMaxLength(50);
                entity.Property(e => e.PricePerHour).HasColumnType("decimal(12,2)");
                entity.Property(e => e.PricePerDay).HasColumnType("decimal(12,2)");

                entity.HasOne(v => v.Station)
                    .WithMany(s => s.Vehicles)
                    .HasForeignKey(v => v.StationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure RentalRecord
            modelBuilder.Entity<RentalRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BasePrice).HasColumnType("decimal(12,2)");
                entity.Property(e => e.DepositFee).HasColumnType("decimal(12,2)");
                entity.Property(e => e.ReservationFee).HasColumnType("decimal(12,2)");
                entity.Property(e => e.ExtraFees).HasColumnType("decimal(12,2)").HasDefaultValue(0);
                entity.Property(e => e.Discount).HasColumnType("decimal(12,2)").HasDefaultValue(0);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(12,2)");
                entity.Property(e => e.OtpCode).HasMaxLength(6);

                // Renter relationship
                entity.HasOne(r => r.Renter)
                    .WithMany(a => a.RentalRecords)
                    .HasForeignKey(r => r.RenterId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Vehicle relationship
                entity.HasOne(r => r.Vehicle)
                    .WithMany(v => v.RentalRecords)
                    .HasForeignKey(r => r.VehicleId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Pickup Station relationship
                entity.HasOne(r => r.PickupStation)
                    .WithMany(s => s.PickupRentalRecords)
                    .HasForeignKey(r => r.PickupStationId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Return Station relationship
                entity.HasOne(r => r.ReturnStation)
                    .WithMany(s => s.ReturnRentalRecords)
                    .HasForeignKey(r => r.ReturnStationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(12,2)").IsRequired();
                entity.Property(e => e.Method).HasMaxLength(50);
                entity.Property(e => e.TransactionRef).HasMaxLength(200);
                entity.Property(e => e.Status).HasMaxLength(30).HasDefaultValue("pending");

                entity.HasOne(p => p.RentalRecord)
                    .WithMany(r => r.Payments)
                    .HasForeignKey(p => p.RentalId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure InspectionProblem
            modelBuilder.Entity<InspectionProblem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IncidentType).HasMaxLength(50);
                entity.Property(e => e.PenaltyAmount).HasColumnType("decimal(12,2)").HasDefaultValue(0);

                entity.HasOne(i => i.RentalRecord)
                    .WithMany(r => r.InspectionProblems)
                    .HasForeignKey(i => i.RentalId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure RatingReview
            modelBuilder.Entity<RatingReview>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(r => r.RentalRecord)
                    .WithOne(rr => rr.RatingReview)
                    .HasForeignKey<RatingReview>(r => r.RentalId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed dữ liệu từ DataSeeder
            DataSeeder.SeedData(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
 }
