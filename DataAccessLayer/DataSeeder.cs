using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public static class DataSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Accounts
            modelBuilder.Entity<Account>().HasData(
                // Admin Account
                new Account
                {
                    Id = 1,
                    FullName = "Administrator",
                    Email = "admin@evrental.com",
                    Phone = "0123456789",
                    PasswordHash = "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", //Admin@123
                    Role = AccountRole.Admin,
                    IsActive = true,
                    CreateDate = new DateTime(2025, 10, 24),
                    UpdateDate = new DateTime(2025, 10, 24),
                    IsDeleted = false
                },
                // Staff Account
                new Account
                {
                    Id = 2,
                    FullName = "Nguyen Van Staff",
                    Email = "staff@evrental.com",
                    Phone = "0987654321",
                    PasswordHash = "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", //Admin@123
                    Role = AccountRole.Staff,
                    IsActive = true,
                    CreateDate = new DateTime(2025, 10, 24),
                    UpdateDate = new DateTime(2025, 10, 24),
                    IsDeleted = false
                },
                // Renter Account 1
                new Account
                {
                    Id = 3,
                    FullName = "Tran Thi Renter",
                    Email = "renter1@gmail.com",
                    Phone = "0901234567",
                    PasswordHash = "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", //Admin@123
                    Role = AccountRole.Renter,
                    IsActive = true,
                    CreateDate = new DateTime(2025, 10, 25),
                    UpdateDate = new DateTime(2025, 10, 25),
                    IsDeleted = false
                },
                // Renter Account 2
                new Account
                {
                    Id = 4,
                    FullName = "Le Van Renter",
                    Email = "renter2@gmail.com",
                    Phone = "0912345678",
                    PasswordHash = "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", //Admin@123
                    Role = AccountRole.Renter,
                    IsActive = true,
                    CreateDate = new DateTime(2025, 10, 25),
                    UpdateDate = new DateTime(2025, 10, 25),
                    IsDeleted = false
                }
            );

            // Seed Stations
            modelBuilder.Entity<Station>().HasData(
                new Station
                {
                    Id = 1,
                    Name = "Trạm Quận 1 - Bến Thành",
                    Address = "123 Lê Lợi, Phường Bến Thành, Quận 1, TP.HCM",
                    State = "Active",
                    CreateDate = new DateTime(2025, 10, 20),
                    UpdateDate = new DateTime(2025, 10, 20),
                    IsDeleted = false
                },
                new Station
                {
                    Id = 2,
                    Name = "Trạm Quận 3 - Cộng Hòa",
                    Address = "456 Cộng Hòa, Phường 13, Quận Tân Bình, TP.HCM",
                    State = "Active",
                    CreateDate = new DateTime(2025, 10, 20),
                    UpdateDate = new DateTime(2025, 10, 20),
                    IsDeleted = false
                },
                new Station
                {
                    Id = 3,
                    Name = "Trạm Quận 7 - Phú Mỹ Hưng",
                    Address = "789 Nguyễn Văn Linh, Phường Tân Phú, Quận 7, TP.HCM",
                    State = "Active",
                    CreateDate = new DateTime(2025, 10, 20),
                    UpdateDate = new DateTime(2025, 10, 20),
                    IsDeleted = false
                },
                new Station
                {
                    Id = 4,
                    Name = "Trạm Thủ Đức - Landmark 81",
                    Address = "208 Nguyễn Hữu Cảnh, Phường 22, TP. Thủ Đức, TP.HCM",
                    State = "Active",
                    CreateDate = new DateTime(2025, 10, 20),
                    UpdateDate = new DateTime(2025, 10, 20),
                    IsDeleted = false
                }
            );

            // Seed Vehicles
            modelBuilder.Entity<Vehicle>().HasData(
                // Vehicles at Station 1
                new Vehicle
                {
                    Id = 1,
                    StationId = 1,
                    Name = "VinFast VF e34",
                    Brand = "VinFast",
                    PlateNumber = "59A-12345",
                    Model = "VF e34",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 150000,
                    PricePerDay = 1200000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true}",
                    ImageUrl = "https://www.vinfastvietnam.net.vn/uploads/data/3097/imgproducts/vinfastvietnam.net.vnvfe34.jpg3.jpg",
                    MaxDistance = 285,
                    CreateDate = new DateTime(2025, 10, 21),
                    UpdateDate = new DateTime(2025, 10, 21),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 2,
                    StationId = 1,
                    Name = "VinFast Klara S",
                    Brand = "VinFast",
                    PlateNumber = "59B-67890",
                    Model = "Klara S",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Available,
                    PricePerHour = 30000,
                    PricePerDay = 200000,
                    Features = "{\"gps\":true,\"insurance\":true}",
                    ImageUrl = "https://shop.vinfastauto.com/on/demandware.static/-/Sites-app_vinfast_vn-Library/default/dw02b7ceb2/images/kara/head-banner.png",
                    MaxDistance = 90,
                    CreateDate = new DateTime(2025, 10, 21),
                    UpdateDate = new DateTime(2025, 10, 21),
                    IsDeleted = false
                },
                // Vehicles at Station 2
                new Vehicle
                {
                    Id = 3,
                    StationId = 2,
                    Name = "DatBike Weaver 200",
                    Brand = "DatBike",
                    PlateNumber = "51A-11111",
                    Model = "Weaver 200",
                    VehicleType = "motorbike",
                    Status = VehicleStatus.Available,
                    PricePerHour = 35000,
                    PricePerDay = 250000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true,\"antiTheft\":true}",
                    ImageUrl = "https://vcdn1-vnexpress.vnecdn.net/2021/11/30/weaver-200-1-1638229818-5666-1638230365.jpg?w=460&h=0&q=100&dpr=2&fit=crop&s=tEMINgIF_iQNoKpS66-pQQ",
                    MaxDistance = 120,
                    CreateDate = new DateTime(2025, 10, 21),
                    UpdateDate = new DateTime(2025, 10, 21),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 4,
                    StationId = 2,
                    Name = "DatBike Weaver 100",
                    Brand = "DatBike",
                    PlateNumber = "51B-22222",
                    Model = "Weaver 100",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Available,
                    PricePerHour = 28000,
                    PricePerDay = 195000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true}",
                    ImageUrl = "https://dat.bike/cdn/shop/files/datbike-1_1728x_ead528e3-8a1c-4de4-89b9-4a567cfa2fdb.jpg?v=1668681506",
                    MaxDistance = 100,
                    CreateDate = new DateTime(2025, 10, 21),
                    UpdateDate = new DateTime(2025, 10, 21),
                    IsDeleted = false
                },
                // Vehicles at Station 3
                new Vehicle
                {
                    Id = 5,
                    StationId = 3,
                    Name = "Tesla Model 3",
                    Brand = "Tesla",
                    PlateNumber = "59C-33333",
                    Model = "Model 3 Standard Range",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 220000,
                    PricePerDay = 1800000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true}",
                    ImageUrl = "https://giaxeoto.vn/admin/upload/images/resize/640-tesla-model-3-2024-co-gi-moi.jpg",
                    MaxDistance = 491,
                    CreateDate = new DateTime(2025, 10, 21),
                    UpdateDate = new DateTime(2025, 10, 21),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 6,
                    StationId = 3,
                    Name = "Yadea G5",
                    Brand = "Yadea",
                    PlateNumber = "59D-44444",
                    Model = "G5 Pro",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Rented,
                    PricePerHour = 22000,
                    PricePerDay = 160000,
                    Features = "{\"gps\":true,\"insurance\":true,\"usbCharging\":true}",
                    ImageUrl = "https://xedienvietthanh.com/wp-content/uploads/2022/01/1-2.jpg",
                    MaxDistance = 80,
                    CreateDate = new DateTime(2025, 10, 21),
                    UpdateDate = new DateTime(2025, 10, 21),
                    IsDeleted = false
                },
                // Vehicles at Station 4
                new Vehicle
                {
                    Id = 7,
                    StationId = 4,
                    Name = "BMW iX3",
                    Brand = "BMW",
                    PlateNumber = "51C-55555",
                    Model = "iX3 M Sport",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 280000,
                    PricePerDay = 2200000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"sunroof\":true,\"leatherSeats\":true,\"panoramicRoof\":true}",
                    ImageUrl = "https://images.netdirector.co.uk/gforces-auto/image/upload/w_412,h_309,q_auto,c_fill,f_auto,fl_lossy/auto-titan/e9fc28a92a2bff98fdb38daeb05779d0/ix3_new_highlights.png",
                    MaxDistance = 460,
                    CreateDate = new DateTime(2025, 10, 21),
                    UpdateDate = new DateTime(2025, 10, 21),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 8,
                    StationId = 4,
                    Name = "Honda EV-neo",
                    Brand = "Honda",
                    PlateNumber = "51D-66666",
                    Model = "EV-neo Electric",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Maintenance,
                    PricePerHour = 26000,
                    PricePerDay = 185000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true}",
                    ImageUrl = "https://imgs.vietnamnet.vn/Images/2010/12/18/15/20101218153440_HondaEV_neo1.jpg?width=0&s=nYxnKbZLc3xcypiHjjRVgA",
                    MaxDistance = 90,
                    CreateDate = new DateTime(2025, 10, 21),
                    UpdateDate = new DateTime(2025, 10, 21),
                    IsDeleted = false
                }
            );
        }
    }
}
