using DataAccessLayer.Entities;
// using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public static class DataSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Accounts
            modelBuilder
                .Entity<Account>()
                .HasData(
                    // Admin Account
                    new Account
                    {
                        Id = 1,
                        FullName = "Administrator",
                        Email = "admin@evrental.com",
                        Phone = "0123456789",
                        PasswordHash =
                            "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", //Admin@123
                        Role = AccountRole.Admin,
                        IsActive = true,
                        CreateDate = new DateTime(2025, 10, 24),
                        UpdateDate = new DateTime(2025, 10, 24),
                        IsDeleted = false,
                    },
                    // Staff Account
                    new Account
                    {
                        Id = 2,
                        FullName = "Nguyen Van Staff",
                        Email = "staff@evrental.com",
                        Phone = "0987654321",
                        PasswordHash =
                            "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", //Admin@123
                        Role = AccountRole.Staff,
                        IsActive = true,
                        CreateDate = new DateTime(2025, 10, 24),
                        UpdateDate = new DateTime(2025, 10, 24),
                        IsDeleted = false,
                    },
                    // Renter Account 1
                    new Account
                    {
                        Id = 3,
                        FullName = "Tran Thi Renter",
                        Email = "renter1@gmail.com",
                        Phone = "0901234567",
                        PasswordHash =
                            "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", //Admin@123
                        Role = AccountRole.Renter,
                        IsActive = true,
                        CreateDate = new DateTime(2025, 10, 25),
                        UpdateDate = new DateTime(2025, 10, 25),
                        IsDeleted = false,
                    },
                    // Renter Account 2
                    new Account
                    {
                        Id = 4,
                        FullName = "Le Van Renter",
                        Email = "renter2@gmail.com",
                        Phone = "0912345678",
                        PasswordHash =
                            "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", //Admin@123
                        Role = AccountRole.Renter,
                        IsActive = true,
                        CreateDate = new DateTime(2025, 10, 25),
                        UpdateDate = new DateTime(2025, 10, 25),
                        IsDeleted = false,
                    }
                );

            // Seed Stations
            modelBuilder
                .Entity<Station>()
                .HasData(
                    new Station
                    {
                        Id = 1,
                        Name = "Trạm Quận 1 - Bến Thành",
                        Address = "123 Lê Lợi, Phường Bến Thành, Quận 1, TP.HCM",
                        State = "Active",
                        CreateDate = new DateTime(2025, 10, 20),
                        UpdateDate = new DateTime(2025, 10, 20),
                        IsDeleted = false,
                    },
                    new Station
                    {
                        Id = 2,
                        Name = "Trạm Quận 3 - Cộng Hòa",
                        Address = "456 Cộng Hòa, Phường 13, Quận Tân Bình, TP.HCM",
                        State = "Active",
                        CreateDate = new DateTime(2025, 10, 20),
                        UpdateDate = new DateTime(2025, 10, 20),
                        IsDeleted = false,
                    },
                    new Station
                    {
                        Id = 3,
                        Name = "Trạm Quận 7 - Phú Mỹ Hưng",
                        Address = "789 Nguyễn Văn Linh, Phường Tân Phú, Quận 7, TP.HCM",
                        State = "Active",
                        CreateDate = new DateTime(2025, 10, 20),
                        UpdateDate = new DateTime(2025, 10, 20),
                        IsDeleted = false,
                    },
                    new Station
                    {
                        Id = 4,
                        Name = "Trạm Thủ Đức - Landmark 81",
                        Address = "208 Nguyễn Hữu Cảnh, Phường 22, TP. Thủ Đức, TP.HCM",
                        State = "Active",
                        CreateDate = new DateTime(2025, 10, 20),
                        UpdateDate = new DateTime(2025, 10, 20),
                        IsDeleted = false,
                    }
                );

            // Seed Vehicles
            modelBuilder
                .Entity<Vehicle>()
                .HasData(
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
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true}",
                        ImageUrl =
                            "https://www.vinfastvietnam.net.vn/uploads/data/3097/imgproducts/vinfastvietnam.net.vnvfe34.jpg3.jpg",
                        MaxDistance = 285,
                        seartCapacity = 5,
                        BatteryCapacity = 37.5m,
                        CreateDate = new DateTime(2025, 10, 21),
                        UpdateDate = new DateTime(2025, 10, 21),
                        IsDeleted = false,
                    },
                    // Vehicles at Station 2
                    new Vehicle
                    {
                        Id = 3,
                        StationId = 2,
                        Name = "Tesla Model 3",
                        Brand = "Tesla",
                        PlateNumber = "59C-33333",
                        Model = "Model 3 Standard Range",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 220000,
                        PricePerDay = 1800000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true}",
                        ImageUrl =
                            "https://giaxeoto.vn/admin/upload/images/resize/640-tesla-model-3-2024-co-gi-moi.jpg",
                        MaxDistance = 491,
                        seartCapacity = 5,
                        BatteryCapacity = 82m,
                        CreateDate = new DateTime(2025, 10, 21),
                        UpdateDate = new DateTime(2025, 10, 21),
                        IsDeleted = false,
                    },
                    // Vehicles at Station 3
                    new Vehicle
                    {
                        Id = 5,
                        StationId = 3,
                        Name = "BMW iX3",
                        Brand = "BMW",
                        PlateNumber = "51C-55555",
                        Model = "iX3 M Sport",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 280000,
                        PricePerDay = 2200000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"sunroof\":true,\"leatherSeats\":true,\"panoramicRoof\":true}",
                        ImageUrl =
                            "https://images.netdirector.co.uk/gforces-auto/image/upload/w_412,h_309,q_auto,c_fill,f_auto,fl_lossy/auto-titan/e9fc28a92a2bff98fdb38daeb05779d0/ix3_new_highlights.png",
                        MaxDistance = 460,
                        seartCapacity = 5,
                        BatteryCapacity = 84m,
                        CreateDate = new DateTime(2025, 10, 21),
                        UpdateDate = new DateTime(2025, 10, 21),
                        IsDeleted = false,
                    },
                    // Vehicles at Station 4
                    new Vehicle
                    {
                        Id = 7,
                        StationId = 4,
                        Name = "Audi e-tron GT",
                        Brand = "Audi",
                        PlateNumber = "51E-99999",
                        Model = "e-tron GT quattro",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 320000,
                        PricePerDay = 2500000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"sportMode\":true}",
                        ImageUrl =
                            "https://i1-vnexpress.vnecdn.net/2024/11/23/DSC09878JPG-1732351567.jpg?w=750&h=450&q=100&dpr=1&fit=crop&s=UVB1kqgA08fA_pGNG7EjvA",
                        MaxDistance = 488,
                        seartCapacity = 4,
                        BatteryCapacity = 93m,
                        CreateDate = new DateTime(2025, 10, 21),
                        UpdateDate = new DateTime(2025, 10, 21),
                        IsDeleted = false,
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
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true}",
                        ImageUrl =
                            "https://vinfast-cars.vn/wp-content/uploads/2025/02/vinfast-vf8-den.png",
                        MaxDistance = 420,
                        seartCapacity = 5,
                        BatteryCapacity = 75m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 11,
                        StationId = 2,
                        Name = "Mercedes EQS",
                        Brand = "Mercedes-Benz",
                        PlateNumber = "59G-22233",
                        Model = "EQS 450+",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 350000,
                        PricePerDay = 2800000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"massage\":true,\"panoramicRoof\":true}",
                        ImageUrl =
                            "https://i1-vnexpress.vnecdn.net/2023/03/29/Mercedes-EQS-2022-VnE-7034-JPG.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=VNrfMglzD7glUa199o-N6A&t=image",
                        MaxDistance = 770,
                        seartCapacity = 5,
                        BatteryCapacity = 107.8m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 13,
                        StationId = 3,
                        Name = "Porsche Taycan",
                        Brand = "Porsche",
                        PlateNumber = "51G-44455",
                        Model = "Taycan 4S",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 380000,
                        PricePerDay = 3000000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"sportMode\":true,\"launch\":true}",
                        ImageUrl =
                            "https://i1-vnexpress.vnecdn.net/2024/10/18/Porsche-Taycan-Vnexpress-net-11-JPG.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=LoskMEDqKHzXgrHyeWd5Ag&t=image",
                        MaxDistance = 484,
                        seartCapacity = 4,
                        BatteryCapacity = 93.2m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 15,
                        StationId = 4,
                        Name = "VinFast VF 9",
                        Brand = "VinFast",
                        PlateNumber = "59I-66677",
                        Model = "VF 9 Plus",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 200000,
                        PricePerDay = 1600000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"7seats\":true,\"panoramicRoof\":true}",
                        ImageUrl =
                            "https://shop.vinfastauto.com/on/demandware.static/-/Sites-app_vinfast_vn-Library/default/dw1a73c862/images/PDP/vf9/202406/exterior/CE1W.webp",
                        MaxDistance = 438,
                        seartCapacity = 7,
                        BatteryCapacity = 92m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 17,
                        StationId = 1,
                        Name = "Tesla Model Y",
                        Brand = "Tesla",
                        PlateNumber = "51I-77788",
                        Model = "Model Y Long Range",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 240000,
                        PricePerDay = 1900000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true,\"7seats\":true}",
                        ImageUrl =
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSQuPPGwtfobtphn2JnZfRLJU_ELJXj4mEweQ&s",
                        MaxDistance = 525,
                        seartCapacity = 7,
                        BatteryCapacity = 82m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 19,
                        StationId = 2,
                        Name = "Hyundai Ioniq 5",
                        Brand = "Hyundai",
                        PlateNumber = "59J-88899",
                        Model = "Ioniq 5 Long Range",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 190000,
                        PricePerDay = 1500000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"solarRoof\":true}",
                        ImageUrl =
                            "https://i1-vnexpress.vnecdn.net/2023/07/31/Hyundai-IONIQ-5-7.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=gqOfVmNy6EZxHps0rNBfCA&t=image",
                        MaxDistance = 481,
                        seartCapacity = 5,
                        BatteryCapacity = 84m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 21,
                        StationId = 3,
                        Name = "Kia EV6",
                        Brand = "Kia",
                        PlateNumber = "51J-99900",
                        Model = "EV6 GT-Line",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 195000,
                        PricePerDay = 1550000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"fastCharging\":true}",
                        ImageUrl =
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTDRNkoUruCHw69jdLnwmvz09ncLCsAcLnsJA&s",
                        MaxDistance = 528,
                        seartCapacity = 5,
                        BatteryCapacity = 84m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 23,
                        StationId = 4,
                        Name = "Polestar 2",
                        Brand = "Polestar",
                        PlateNumber = "59L-33322",
                        Model = "Polestar 2 Long Range",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 230000,
                        PricePerDay = 1850000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"googleIntegration\":true}",
                        ImageUrl =
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTFP_xRoDnlB2qUUnef4lw1c-HTf7Xnvi_hWw&s",
                        MaxDistance = 540,
                        seartCapacity = 5,
                        BatteryCapacity = 100m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 25,
                        StationId = 1,
                        Name = "BYD Atto 3",
                        Brand = "BYD",
                        PlateNumber = "51M-66655",
                        Model = "Atto 3 Extended",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 185000,
                        PricePerDay = 1450000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"panoramicSunroof\":true}",
                        ImageUrl = "https://img1.oto.com.vn/2024/07/26/OpzfnMD2/atto-3-0f7e.webp",
                        MaxDistance = 480,
                        seartCapacity = 5,
                        BatteryCapacity = 60.48m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 27,
                        StationId = 2,
                        Name = "Nissan Ariya",
                        Brand = "Nissan",
                        PlateNumber = "51L-44433",
                        Model = "Ariya e-4ORCE",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 210000,
                        PricePerDay = 1700000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"proPilot\":true}",
                        ImageUrl =
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTNR4feHONvrN5Y-HK13689YKvLgkYgtqWiyA&s",
                        MaxDistance = 500,
                        seartCapacity = 5,
                        BatteryCapacity = 87m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    // Additional Vehicles - 16 more cars
                    new Vehicle
                    {
                        Id = 28,
                        StationId = 3,
                        Name = "Lexus ES 300h",
                        Brand = "Lexus",
                        PlateNumber = "59N-11234",
                        Model = "ES 300h Hybrid",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 250000,
                        PricePerDay = 2000000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"markLevinson\":true}",
                        ImageUrl =
                            "https://tse1.mm.bing.net/th/id/OIP.EXoP1hIJLIFEKo0FkHEWaAHaE7?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 565,
                        seartCapacity = 5,
                        BatteryCapacity = 112m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 29,
                        StationId = 4,
                        Name = "Volvo XC60",
                        Brand = "Volvo",
                        PlateNumber = "51N-22345",
                        Model = "XC60 Recharge",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 240000,
                        PricePerDay = 1900000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"panoramicSunroof\":true,\"safetyFeatures\":true}",
                        ImageUrl =
                            "https://images.carexpert.com.au/resize/3000/-/app/uploads/2024/05/Volvo-XC60-Black-Edition-MY24-Stills-9.jpg",
                        MaxDistance = 470,
                        seartCapacity = 5,
                        BatteryCapacity = 78m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 30,
                        StationId = 1,
                        Name = "Jaguar I-PACE",
                        Brand = "Jaguar",
                        PlateNumber = "59O-33456",
                        Model = "I-PACE SE",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 270000,
                        PricePerDay = 2150000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"premiumAudio\":true,\"fastCharging\":true}",
                        ImageUrl =
                            "https://tse1.mm.bing.net/th/id/OIP.fDbVDlTRPxQPvFrCIs5zQQHaEK?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 480,
                        seartCapacity = 5,
                        BatteryCapacity = 100m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 31,
                        StationId = 2,
                        Name = "Chevrolet Bolt EV",
                        Brand = "Chevrolet",
                        PlateNumber = "51O-44567",
                        Model = "Bolt EV",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 160000,
                        PricePerDay = 1280000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"backupCamera\":true}",
                        ImageUrl =
                            "https://tse3.mm.bing.net/th/id/OIP.5tjOrCPF-Yl7obuAiHgaqAHaE8?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 417,
                        seartCapacity = 5,
                        BatteryCapacity = 66m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 32,
                        StationId = 3,
                        Name = "Volkswagen ID.Buzz",
                        Brand = "Volkswagen",
                        PlateNumber = "59P-55678",
                        Model = "ID.Buzz Pro",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 280000,
                        PricePerDay = 2200000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"spacious\":true,\"familyFriendly\":true}",
                        ImageUrl =
                            "https://tse2.mm.bing.net/th/id/OIP.M1rRxlD1pU1PIvRiRQg9CwHaEK?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 450,
                        seartCapacity = 7,
                        BatteryCapacity = 82m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 33,
                        StationId = 4,
                        Name = "Lucid Air",
                        Brand = "Lucid",
                        PlateNumber = "51P-66789",
                        Model = "Air Touring",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 400000,
                        PricePerDay = 3200000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"luxuryInterior\":true,\"advancedTech\":true}",
                        ImageUrl =
                            "https://tse1.mm.bing.net/th/id/OIP.crKvBvznDwyNtAA4dgXrOQHaEK?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 650,
                        seartCapacity = 5,
                        BatteryCapacity = 112.5m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 34,
                        StationId = 1,
                        Name = "Fisker Ocean",
                        Brand = "Fisker",
                        PlateNumber = "59Q-77890",
                        Model = "Ocean Extreme",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 200000,
                        PricePerDay = 1600000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"solarPanel\":true,\"sustainableDesign\":true}",
                        ImageUrl =
                            "https://tse1.mm.bing.net/th/id/OIP.ykCFd2ydTNFE6EhyNnU8JQHaEK?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 440,
                        seartCapacity = 5,
                        BatteryCapacity = 112m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 35,
                        StationId = 2,
                        Name = "Genesis GV70",
                        Brand = "Genesis",
                        PlateNumber = "51Q-88901",
                        Model = "GV70 Electrified",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 270000,
                        PricePerDay = 2150000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"premiumAudio\":true,\"luxuryBrand\":true}",
                        ImageUrl =
                            "https://tse4.mm.bing.net/th/id/OIP.hhVPMkVGLIQbrjDlLLow8QHaEK?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 480,
                        seartCapacity = 5,
                        BatteryCapacity = 99m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 36,
                        StationId = 3,
                        Name = "Subaru Solterra",
                        Brand = "Subaru",
                        PlateNumber = "59R-99012",
                        Model = "Solterra Premium",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 220000,
                        PricePerDay = 1750000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"awd\":true,\"offRoad\":true}",
                        ImageUrl =
                            "https://tse4.mm.bing.net/th/id/OIP.LWT73M5Yi10RQELHGjZPlwHaEK?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 460,
                        seartCapacity = 5,
                        BatteryCapacity = 71.4m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 37,
                        StationId = 4,
                        Name = "Lotus Eletre",
                        Brand = "Lotus",
                        PlateNumber = "51R-00123",
                        Model = "Eletre Sport",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 290000,
                        PricePerDay = 2300000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"sportDesign\":true,\"performanceMode\":true}",
                        ImageUrl =
                            "https://tse1.mm.bing.net/th/id/OIP.pJwMoDYgPejxlRbwiKxASQHaE8?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 520,
                        seartCapacity = 5,
                        BatteryCapacity = 112m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 38,
                        StationId = 1,
                        Name = "Mazda MX-30",
                        Brand = "Mazda",
                        PlateNumber = "59S-11234",
                        Model = "MX-30",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 170000,
                        PricePerDay = 1360000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"stylishDesign\":true}",
                        ImageUrl =
                            "https://tse3.mm.bing.net/th/id/OIP.TLhC40ExkQ012Ox3kH31jgHaEK?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 290,
                        seartCapacity = 5,
                        BatteryCapacity = 35.5m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 39,
                        StationId = 2,
                        Name = "Renault 5 Turbo 3E",
                        Brand = "Renault",
                        PlateNumber = "51S-22345",
                        Model = "5 Turbo 3E",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 180000,
                        PricePerDay = 1440000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"retroDesign\":true,\"sporty\":true}",
                        ImageUrl =
                            "https://tse2.mm.bing.net/th/id/OIP.E5ZVq8AWKMIfhecu97Yh_AHaEK?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 380,
                        seartCapacity = 4,
                        BatteryCapacity = 52m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 40,
                        StationId = 3,
                        Name = "Fiat 500e",
                        Brand = "Fiat",
                        PlateNumber = "59T-33456",
                        Model = "500e",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 140000,
                        PricePerDay = 1120000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"compact\":true,\"cityFriendly\":true}",
                        ImageUrl =
                            "https://cdn.motor1.com/images/mgl/L3kyjQ/s1/2024-fiat-500e-first-drive-review.jpg",
                        MaxDistance = 330,
                        seartCapacity = 4,
                        BatteryCapacity = 42m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 41,
                        StationId = 4,
                        Name = "MINI Cooper SE",
                        Brand = "MINI",
                        PlateNumber = "51T-44567",
                        Model = "Cooper SE",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 155000,
                        PricePerDay = 1240000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"compact\":true,\"fun\":true}",
                        ImageUrl =
                            "https://th.bing.com/th/id/OIP.HoFXfZQFuzUODYGEbBluVgHaE8?w=249&h=180&c=7&r=0&o=7&cb=ucfimg2&pid=1.7&rm=3&ucfimg=1",
                        MaxDistance = 270,
                        seartCapacity = 4,
                        BatteryCapacity = 54m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 42,
                        StationId = 1,
                        Name = "Opel Grandland",
                        Brand = "Opel",
                        PlateNumber = "59U-55678",
                        Model = "Grandland Electric",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 200000,
                        PricePerDay = 1600000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"spacious\":true,\"practical\":true}",
                        ImageUrl =
                            "https://tse3.mm.bing.net/th/id/OIP.Y0f4-n6c7edbPlQZ_RyrkAHaE8?cb=ucfimg2ucfimg=1&rs=1&pid=ImgDetMain&o=7&rm=3",
                        MaxDistance = 440,
                        seartCapacity = 5,
                        BatteryCapacity = 77m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    },
                    new Vehicle
                    {
                        Id = 43,
                        StationId = 2,
                        Name = "Cupra Born",
                        Brand = "Cupra",
                        PlateNumber = "51U-66789",
                        Model = "Born e-Boost",
                        VehicleType = "car",
                        Status = VehicleStatus.Available,
                        PricePerHour = 210000,
                        PricePerDay = 1680000,
                        Features =
                            "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"sportStyle\":true,\"performance\":true}",
                        ImageUrl =
                            "https://www.cupraofficial.com/content/dam/public/cupra-website/cars/car-range/new-cupra-born-aurora-blue-car.png",
                        MaxDistance = 500,
                        seartCapacity = 5,
                        BatteryCapacity = 82m,
                        CreateDate = new DateTime(2025, 10, 22),
                        UpdateDate = new DateTime(2025, 10, 22),
                        IsDeleted = false,
                    }
                );
        }
    }
}
