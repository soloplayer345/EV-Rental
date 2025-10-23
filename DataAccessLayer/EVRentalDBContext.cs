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

        public DbSet<Renter> Renters { get; set; }

        public DbSet<Staff> Staffs { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<RentalRecord> RentalRecords { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Station> Stations { get; set; }

        public DbSet<VehicleCheck> VehicleChecks { get; set; }

        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mối quan hệ 1-1 giữa Account và Renter
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Renter)
                .WithOne(r => r.Account)
                .HasForeignKey<Renter>(r => r.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            // Cấu hình mối quan hệ 1-1 giữa Account và Staff
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Staff)
                .WithOne(s => s.Account)
                .HasForeignKey<Staff>(s => s.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            // Cấu hình FK StationId cho Report -> Station
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Station)
                .WithMany()
                .HasForeignKey(r => r.StationId)
                .OnDelete(DeleteBehavior.NoAction);

            // Cấu hình FK StationId cho Staff -> Station
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Station)
                .WithMany(st => st.Staff)
                .HasForeignKey(s => s.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình FK StationId cho Vehicle -> Station
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Station)
                .WithMany(st => st.Vehicles)
                .HasForeignKey(v => v.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình FK StationId cho RentalRecord -> Station
            modelBuilder.Entity<RentalRecord>()
                .HasOne(r => r.Station)
                .WithMany(st => st.RentalRecords)
                .HasForeignKey(r => r.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình FK RenterId cho RentalRecord -> Renter
            modelBuilder.Entity<RentalRecord>()
                .HasOne(r => r.Renter)
                .WithMany(rn => rn.RentalRecords)
                .HasForeignKey(r => r.RenterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình FK VehicleId cho RentalRecord -> Vehicle
            modelBuilder.Entity<RentalRecord>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.RentalRecords)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình FK RentalId cho Payment -> RentalRecord
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.RentalRecord)
                .WithMany(r => r.Payments)
                .HasForeignKey(p => p.RentalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình FK RentalId cho VehicleCheck -> RentalRecord
            modelBuilder.Entity<VehicleCheck>()
                .HasOne(vc => vc.RentalRecord)
                .WithMany(r => r.VehicleChecks)
                .HasForeignKey(vc => vc.RentalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình FK StaffId cho VehicleCheck -> Staff
            modelBuilder.Entity<VehicleCheck>()
                .HasOne(vc => vc.Staff)
                .WithMany(s => s.VehicleChecks)
                .HasForeignKey(vc => vc.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed dữ liệu từ DataSeeder
            DataSeeder.SeedData(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
 }
