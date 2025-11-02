using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PricePerDay = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxDistance = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RentalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RenterId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    PickupStationId = table.Column<int>(type: "int", nullable: false),
                    ReturnStationId = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DepositFee = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ReservationFee = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ExtraFees = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    Discount = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    TotalPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalRecords_Account_RenterId",
                        column: x => x.RenterId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalRecords_Stations_PickupStationId",
                        column: x => x.PickupStationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalRecords_Stations_ReturnStationId",
                        column: x => x.ReturnStationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalRecords_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionProblems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalId = table.Column<int>(type: "int", nullable: false),
                    IncidentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Evidence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionProblems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionProblems_RentalRecords_RentalId",
                        column: x => x.RentalId,
                        principalTable: "RentalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "pending"),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_RentalRecords_RentalId",
                        column: x => x.RentalId,
                        principalTable: "RentalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RatingReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatingReviews_RentalRecords_RentalId",
                        column: x => x.RentalId,
                        principalTable: "RentalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Phone", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@evrental.com", "Administrator", true, "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", "0123456789", 2, new DateTime(2025, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "staff@evrental.com", "Nguyen Van Staff", true, "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", "0987654321", 1, new DateTime(2025, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "renter1@gmail.com", "Tran Thi Renter", true, "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", "0901234567", 0, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "renter2@gmail.com", "Le Van Renter", true, "$2a$12$cCtbISaErU6KdR09HUiVvuX0Ur2saXpU/sEB3sQJ72obhp8jKbCa.", "0912345678", 0, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "CreateDate", "IsDeleted", "Name", "State", "UpdateDate" },
                values: new object[,]
                {
                    { 1, "123 Lê Lợi, Phường Bến Thành, Quận 1, TP.HCM", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Trạm Quận 1 - Bến Thành", "Active", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "456 Cộng Hòa, Phường 13, Quận Tân Bình, TP.HCM", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Trạm Quận 3 - Cộng Hòa", "Active", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "789 Nguyễn Văn Linh, Phường Tân Phú, Quận 7, TP.HCM", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Trạm Quận 7 - Phú Mỹ Hưng", "Active", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "208 Nguyễn Hữu Cảnh, Phường 22, TP. Thủ Đức, TP.HCM", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Trạm Thủ Đức - Landmark 81", "Active", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "Brand", "CreateDate", "Features", "ImageUrl", "IsDeleted", "MaxDistance", "Model", "Name", "PlateNumber", "PricePerDay", "PricePerHour", "StationId", "Status", "UpdateDate", "VehicleType" },
                values: new object[,]
                {
                    { 1, "VinFast", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true}", "https://www.vinfastvietnam.net.vn/uploads/data/3097/imgproducts/vinfastvietnam.net.vnvfe34.jpg3.jpg", false, 285, "VF e34", "VinFast VF e34", "59A-12345", 1200000m, 150000m, 1, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 2, "VinFast", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true}", "https://shop.vinfastauto.com/on/demandware.static/-/Sites-app_vinfast_vn-Library/default/dw02b7ceb2/images/kara/head-banner.png", false, 90, "Klara S", "VinFast Klara S", "59B-67890", 200000m, 30000m, 1, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 3, "DatBike", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true,\"antiTheft\":true}", "https://vcdn1-vnexpress.vnecdn.net/2021/11/30/weaver-200-1-1638229818-5666-1638230365.jpg?w=460&h=0&q=100&dpr=2&fit=crop&s=tEMINgIF_iQNoKpS66-pQQ", false, 120, "Weaver 200", "DatBike Weaver 200", "51A-11111", 250000m, 35000m, 2, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "motorbike" },
                    { 4, "DatBike", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true}", "https://dat.bike/cdn/shop/files/datbike-1_1728x_ead528e3-8a1c-4de4-89b9-4a567cfa2fdb.jpg?v=1668681506", false, 100, "Weaver 100", "DatBike Weaver 100", "51B-22222", 195000m, 28000m, 2, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 5, "Tesla", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true}", "https://giaxeoto.vn/admin/upload/images/resize/640-tesla-model-3-2024-co-gi-moi.jpg", false, 491, "Model 3 Standard Range", "Tesla Model 3", "59C-33333", 1800000m, 220000m, 3, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 6, "Yadea", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"usbCharging\":true}", "https://xedienvietthanh.com/wp-content/uploads/2022/01/1-2.jpg", false, 80, "G5 Pro", "Yadea G5", "59D-44444", 160000m, 22000m, 3, 1, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 7, "BMW", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"sunroof\":true,\"leatherSeats\":true,\"panoramicRoof\":true}", "https://images.netdirector.co.uk/gforces-auto/image/upload/w_412,h_309,q_auto,c_fill,f_auto,fl_lossy/auto-titan/e9fc28a92a2bff98fdb38daeb05779d0/ix3_new_highlights.png", false, 460, "iX3 M Sport", "BMW iX3", "51C-55555", 2200000m, 280000m, 4, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 8, "Honda", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true}", "https://imgs.vietnamnet.vn/Images/2010/12/18/15/20101218153440_HondaEV_neo1.jpg?width=0&s=nYxnKbZLc3xcypiHjjRVgA", false, 90, "EV-neo Electric", "Honda EV-neo", "51D-66666", 185000m, 26000m, 4, 2, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 9, "VinFast", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true}", "https://vinfastauto.com/sites/default/files/2022-09/vf-8-xanh-titan.png", false, 420, "VF 8 Plus", "VinFast VF 8", "59E-77777", 1400000m, 180000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 10, "Pega", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"usbCharging\":true}", "https://pega.com.vn/wp-content/uploads/2021/03/pega-xmen-110-trang.jpg", false, 85, "Xmen 110", "Pega Xmen", "59F-88888", 175000m, 25000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 11, "Audi", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"sportMode\":true}", "https://www.audi.com.vn/content/dam/nemo/vn/models/e-tron-gt/my-2021/1920x1080-gallery/1920x1080_A218031.jpg", false, 488, "e-tron GT quattro", "Audi e-tron GT", "51E-99999", 2500000m, 320000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 12, "Yamaha", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true}", "https://cdn.motor1.com/images/mgl/BXxOz/s1/yamaha-e01.jpg", false, 110, "E01 Electric", "Yamaha E01", "51F-11122", 270000m, 38000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "motorbike" },
                    { 13, "Mercedes-Benz", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"massage\":true,\"panoramicRoof\":true}", "https://www.mercedes-benz.com.vn/content/vietnam/vi/passengercars/models/sedan/eqs/overview/_jcr_content/root/responsivegrid/media_slider/media_0/image.component.damq6.3371542717387.jpg", false, 770, "EQS 450+", "Mercedes EQS", "59G-22233", 2800000m, 350000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 14, "Gogoro", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"batterySwap\":true}", "https://www.gogoro.com/tw/media/catalog/product/cache/9/image/9df78eab33525d08d6e5fb8d27136e95/2/p/2plus-white.png", false, 110, "Gogoro 2 Plus", "Gogoro 2", "59H-33344", 220000m, 32000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 15, "Porsche", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"sportMode\":true,\"launch\":true}", "https://files.porsche.com/filestore/image/multimedia/none/992-taycan-modelimage-sideshot/model/cfbb8ed7-1b03-11ea-80c4-005056bbdc38/porsche-model.png", false, 484, "Taycan 4S", "Porsche Taycan", "51G-44455", 3000000m, 380000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 16, "Pega", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true}", "https://pega.com.vn/wp-content/uploads/2022/05/xmen-gt-do.jpg", false, 95, "Xmen GT Pro", "Xmen GT", "51H-55566", 190000m, 28000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 17, "VinFast", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"7seats\":true,\"panoramicRoof\":true}", "https://vinfastauto.com/sites/default/files/2023-03/VF9-xanh.png", false, 438, "VF 9 Plus", "VinFast VF 9", "59I-66677", 1600000m, 200000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 18, "Tesla", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true,\"7seats\":true}", "https://www.tesla.com/sites/default/files/modelsx-new/social/model-y-hero-social.jpg", false, 525, "Model Y Long Range", "Tesla Model Y", "51I-77788", 1900000m, 240000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 19, "Hyundai", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"solarRoof\":true}", "https://www.hyundai.com/content/dam/hyundai/vn/vi/data/find-a-car/ioniq5/highlights/ioniq5-exterior-01.jpg", false, 481, "Ioniq 5 Long Range", "Hyundai Ioniq 5", "59J-88899", 1500000m, 190000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 20, "Kia", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"fastCharging\":true}", "https://www.kia.com/content/dam/kia/vn/vi/images/showroom/ev6/highlights/kia-ev6-highlight-exterior.jpg", false, 528, "EV6 GT-Line", "Kia EV6", "51J-99900", 1550000m, 195000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 21, "DatBike", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true,\"antiTheft\":true}", "https://dat.bike/cdn/shop/files/datbike-weaver-300.jpg?v=1668681506", false, 150, "Weaver 300 Pro", "DatBike Weaver 300", "59K-11100", 280000m, 40000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "motorbike" },
                    { 22, "Yadea", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true,\"usbCharging\":true}", "https://yadeavietnam.com.vn/wp-content/uploads/2021/09/c1s-xanh.png", false, 75, "C1S Smart", "Yadea C1S", "51K-22211", 170000m, 24000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 23, "Polestar", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"googleIntegration\":true}", "https://www.polestar.com/dato/1660211456-polestar-2-hero.png", false, 540, "Polestar 2 Long Range", "Polestar 2", "59L-33322", 1850000m, 230000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 24, "Nissan", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"proPilot\":true}", "https://www.nissan.com.vn/content/dam/Nissan/vn/vehicles/ariya/ariya-hero.jpg", false, 500, "Ariya e-4ORCE", "Nissan Ariya", "51L-44433", 1700000m, 210000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" },
                    { 25, "VinFast", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"smartKey\":true}", "https://shop.vinfastauto.com/on/demandware.static/-/Sites-app_vinfast_vn-Library/default/dw123/images/evo200/evo200-hero.png", false, 85, "Evo200 Lite", "VinFast Evo200", "59M-55544", 185000m, 27000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "scooter" },
                    { 26, "BYD", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"panoramicSunroof\":true}", "https://www.byd.com/content/dam/byd-site/vn/atto3/atto3-exterior.jpg", false, 480, "Atto 3 Extended", "BYD Atto 3", "51M-66655", 1450000m, 185000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Email",
                table: "Account",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionProblems_RentalId",
                table: "InspectionProblems",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RentalId",
                table: "Payments",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingReviews_RentalId",
                table: "RatingReviews",
                column: "RentalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalRecords_PickupStationId",
                table: "RentalRecords",
                column: "PickupStationId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRecords_RenterId",
                table: "RentalRecords",
                column: "RenterId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRecords_ReturnStationId",
                table: "RentalRecords",
                column: "ReturnStationId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRecords_VehicleId",
                table: "RentalRecords",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_StationId",
                table: "Vehicles",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionProblems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RatingReviews");

            migrationBuilder.DropTable(
                name: "RentalRecords");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
