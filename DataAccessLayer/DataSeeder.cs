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
                },
                // Additional Vehicles - Station 1
                new Vehicle
                {
                    Id = 9,
                    StationId = 1,
                    Name = "VinFast VF 8",
                    Brand = "VinFast",
                    PlateNumber = "59E-77777",
                    Model = "VF 8 Plus",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 180000,
                    PricePerDay = 1400000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true}",
                    ImageUrl = "https://vinfast-cars.vn/wp-content/uploads/2025/02/vinfast-vf8-den.png",
                    MaxDistance = 420,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 10,
                    StationId = 1,
                    Name = "Pega Xmen",
                    Brand = "Pega",
                    PlateNumber = "59F-88888",
                    Model = "Xmen 110",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Available,
                    PricePerHour = 25000,
                    PricePerDay = 175000,
                    Features = "{\"gps\":true,\"insurance\":true,\"usbCharging\":true}",
                    ImageUrl = "https://thegioixechaydien.com.vn/upload/hinhanh/xe-may-dien-pega-xmen-350.jpg",
                    MaxDistance = 85,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 11,
                    StationId = 2,
                    Name = "Audi e-tron GT",
                    Brand = "Audi",
                    PlateNumber = "51E-99999",
                    Model = "e-tron GT quattro",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 320000,
                    PricePerDay = 2500000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"sportMode\":true}",
                    ImageUrl = "https://i1-vnexpress.vnecdn.net/2024/11/23/DSC09878JPG-1732351567.jpg?w=750&h=450&q=100&dpr=1&fit=crop&s=UVB1kqgA08fA_pGNG7EjvA",
                    MaxDistance = 488,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 12,
                    StationId = 2,
                    Name = "Yamaha E01",
                    Brand = "Yamaha",
                    PlateNumber = "51F-11122",
                    Model = "E01 Electric",
                    VehicleType = "motorbike",
                    Status = VehicleStatus.Available,
                    PricePerHour = 38000,
                    PricePerDay = 270000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true}",
                    ImageUrl = "https://xedoisong.vn/uploads/20221030/xedoisong_chi_tiet_mau_xe_tay_ga_thuan_dien_yamaha_e01_1__bggd.jpg",
                    MaxDistance = 110,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 13,
                    StationId = 3,
                    Name = "Mercedes EQS",
                    Brand = "Mercedes-Benz",
                    PlateNumber = "59G-22233",
                    Model = "EQS 450+",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 350000,
                    PricePerDay = 2800000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"massage\":true,\"panoramicRoof\":true}",
                    ImageUrl = "https://i1-vnexpress.vnecdn.net/2023/03/29/Mercedes-EQS-2022-VnE-7034-JPG.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=VNrfMglzD7glUa199o-N6A&t=image",
                    MaxDistance = 770,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 14,
                    StationId = 3,
                    Name = "Gogoro 2",
                    Brand = "Gogoro",
                    PlateNumber = "59H-33344",
                    Model = "Gogoro 2 Plus",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Available,
                    PricePerHour = 32000,
                    PricePerDay = 220000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"batterySwap\":true}",
                    ImageUrl = "https://autopro8.mediacdn.vn/2017/-1495870369869.jpg",
                    MaxDistance = 110,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 15,
                    StationId = 4,
                    Name = "Porsche Taycan",
                    Brand = "Porsche",
                    PlateNumber = "51G-44455",
                    Model = "Taycan 4S",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 380000,
                    PricePerDay = 3000000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"sportMode\":true,\"launch\":true}",
                    ImageUrl = "https://i1-vnexpress.vnecdn.net/2024/10/18/Porsche-Taycan-Vnexpress-net-11-JPG.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=LoskMEDqKHzXgrHyeWd5Ag&t=image",
                    MaxDistance = 484,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 16,
                    StationId = 4,
                    Name = "Xmen GT",
                    Brand = "Pega",
                    PlateNumber = "51H-55566",
                    Model = "Xmen GT Pro",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Available,
                    PricePerHour = 28000,
                    PricePerDay = 190000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true}",
                    ImageUrl = "https://xedienducanh.com/upload/product/xe-may-dien-xmen-gt13030.jpg",
                    MaxDistance = 95,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 17,
                    StationId = 1,
                    Name = "VinFast VF 9",
                    Brand = "VinFast",
                    PlateNumber = "59I-66677",
                    Model = "VF 9 Plus",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 200000,
                    PricePerDay = 1600000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"7seats\":true,\"panoramicRoof\":true}",
                    ImageUrl = "https://shop.vinfastauto.com/on/demandware.static/-/Sites-app_vinfast_vn-Library/default/dw1a73c862/images/PDP/vf9/202406/exterior/CE1W.webp",
                    MaxDistance = 438,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 18,
                    StationId = 2,
                    Name = "Tesla Model Y",
                    Brand = "Tesla",
                    PlateNumber = "51I-77788",
                    Model = "Model Y Long Range",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 240000,
                    PricePerDay = 1900000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true,\"7seats\":true}",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSQuPPGwtfobtphn2JnZfRLJU_ELJXj4mEweQ&s",
                    MaxDistance = 525,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 19,
                    StationId = 3,
                    Name = "Hyundai Ioniq 5",
                    Brand = "Hyundai",
                    PlateNumber = "59J-88899",
                    Model = "Ioniq 5 Long Range",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 190000,
                    PricePerDay = 1500000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"solarRoof\":true}",
                    ImageUrl = "https://i1-vnexpress.vnecdn.net/2023/07/31/Hyundai-IONIQ-5-7.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=gqOfVmNy6EZxHps0rNBfCA&t=image",
                    MaxDistance = 481,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 20,
                    StationId = 4,
                    Name = "Kia EV6",
                    Brand = "Kia",
                    PlateNumber = "51J-99900",
                    Model = "EV6 GT-Line",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 195000,
                    PricePerDay = 1550000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"fastCharging\":true}",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTDRNkoUruCHw69jdLnwmvz09ncLCsAcLnsJA&s",
                    MaxDistance = 528,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 21,
                    StationId = 1,
                    Name = "DatBike Weaver 300",
                    Brand = "DatBike",
                    PlateNumber = "59K-11100",
                    Model = "Weaver 300 Pro",
                    VehicleType = "motorbike",
                    Status = VehicleStatus.Available,
                    PricePerHour = 40000,
                    PricePerDay = 280000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true,\"antiTheft\":true}",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS4I4PWW4787JMocGpiEGGCSx1vybQqpptKWg&s",
                    MaxDistance = 150,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 22,
                    StationId = 2,
                    Name = "Yadea C1S",
                    Brand = "Yadea",
                    PlateNumber = "51K-22211",
                    Model = "C1S Smart",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Available,
                    PricePerHour = 24000,
                    PricePerDay = 170000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true}",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTDSr3w-Z6dXCDd1X2htVbLBP6VL4YXEd-Ofw&s",
                    MaxDistance = 75,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 23,
                    StationId = 3,
                    Name = "Polestar 2",
                    Brand = "Polestar",
                    PlateNumber = "59L-33322",
                    Model = "Polestar 2 Long Range",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 230000,
                    PricePerDay = 1850000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"googleIntegration\":true}",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTFP_xRoDnlB2qUUnef4lw1c-HTf7Xnvi_hWw&s",
                    MaxDistance = 540,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 24,
                    StationId = 4,
                    Name = "Nissan Ariya",
                    Brand = "Nissan",
                    PlateNumber = "51L-44433",
                    Model = "Ariya e-4ORCE",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 210000,
                    PricePerDay = 1700000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"proPilot\":true}",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTNR4feHONvrN5Y-HK13689YKvLgkYgtqWiyA&s",
                    MaxDistance = 500,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 25,
                    StationId = 1,
                    Name = "VinFast Evo200",
                    Brand = "VinFast",
                    PlateNumber = "59M-55544",
                    Model = "Evo200 Lite",
                    VehicleType = "scooter",
                    Status = VehicleStatus.Available,
                    PricePerHour = 27000,
                    PricePerDay = 185000,
                    Features = "{\"gps\":true,\"insurance\":true,\"smartKey\":true}",
                    ImageUrl = "https://shop.vinfastauto.com/on/demandware.static/-/Sites-app_vinfast_vn-Library/default/dw0b27b768/images/PDP-XMD/evo200/img-evo-red.png",
                    MaxDistance = 85,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                },
                new Vehicle
                {
                    Id = 26,
                    StationId = 2,
                    Name = "BYD Atto 3",
                    Brand = "BYD",
                    PlateNumber = "51M-66655",
                    Model = "Atto 3 Extended",
                    VehicleType = "car",
                    Status = VehicleStatus.Available,
                    PricePerHour = 185000,
                    PricePerDay = 1450000,
                    Features = "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"panoramicSunroof\":true}",
                    ImageUrl = "https://img1.oto.com.vn/2024/07/26/OpzfnMD2/atto-3-0f7e.webp",
                    MaxDistance = 480,
                    CreateDate = new DateTime(2025, 10, 22),
                    UpdateDate = new DateTime(2025, 10, 22),
                    IsDeleted = false
                }
            );
        }
    }
}
