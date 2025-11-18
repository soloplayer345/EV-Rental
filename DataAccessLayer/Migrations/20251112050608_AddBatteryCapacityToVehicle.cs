using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddBatteryCapacityToVehicle : Migration
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
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VehicleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PricePerDay = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxDistance = table.Column<int>(type: "int", nullable: false),
                    seartCapacity = table.Column<int>(type: "int", nullable: false),
                    BatteryCapacity = table.Column<decimal>(type: "decimal(6,2)", nullable: false, defaultValue: 0m),
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
                columns: new[] { "Id", "BatteryCapacity", "Brand", "CreateDate", "Features", "ImageUrl", "IsDeleted", "MaxDistance", "Model", "Name", "PlateNumber", "PricePerDay", "PricePerHour", "StationId", "Status", "UpdateDate", "VehicleType", "seartCapacity" },
                values: new object[,]
                {
                    { 1, 37.5m, "VinFast", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true}", "https://www.vinfastvietnam.net.vn/uploads/data/3097/imgproducts/vinfastvietnam.net.vnvfe34.jpg3.jpg", false, 285, "VF e34", "VinFast VF e34", "59A-12345", 1200000m, 150000m, 1, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 3, 82m, "Tesla", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true}", "https://giaxeoto.vn/admin/upload/images/resize/640-tesla-model-3-2024-co-gi-moi.jpg", false, 491, "Model 3 Standard Range", "Tesla Model 3", "59C-33333", 1800000m, 220000m, 2, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 5, 84m, "BMW", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"sunroof\":true,\"leatherSeats\":true,\"panoramicRoof\":true}", "https://images.netdirector.co.uk/gforces-auto/image/upload/w_412,h_309,q_auto,c_fill,f_auto,fl_lossy/auto-titan/e9fc28a92a2bff98fdb38daeb05779d0/ix3_new_highlights.png", false, 460, "iX3 M Sport", "BMW iX3", "51C-55555", 2200000m, 280000m, 3, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 7, 93m, "Audi", new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"sportMode\":true}", "https://i1-vnexpress.vnecdn.net/2024/11/23/DSC09878JPG-1732351567.jpg?w=750&h=450&q=100&dpr=1&fit=crop&s=UVB1kqgA08fA_pGNG7EjvA", false, 488, "e-tron GT quattro", "Audi e-tron GT", "51E-99999", 2500000m, 320000m, 4, 0, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 4 },
                    { 9, 75m, "VinFast", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true}", "https://vinfast-cars.vn/wp-content/uploads/2025/02/vinfast-vf8-den.png", false, 420, "VF 8 Plus", "VinFast VF 8", "59E-77777", 1400000m, 180000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 11, 107.8m, "Mercedes-Benz", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"massage\":true,\"panoramicRoof\":true}", "https://i1-vnexpress.vnecdn.net/2023/03/29/Mercedes-EQS-2022-VnE-7034-JPG.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=VNrfMglzD7glUa199o-N6A&t=image", false, 770, "EQS 450+", "Mercedes EQS", "59G-22233", 2800000m, 350000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 13, 93.2m, "Porsche", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"sportMode\":true,\"launch\":true}", "https://i1-vnexpress.vnecdn.net/2024/10/18/Porsche-Taycan-Vnexpress-net-11-JPG.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=LoskMEDqKHzXgrHyeWd5Ag&t=image", false, 484, "Taycan 4S", "Porsche Taycan", "51G-44455", 3000000m, 380000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 4 },
                    { 15, 92m, "VinFast", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"7seats\":true,\"panoramicRoof\":true}", "https://shop.vinfastauto.com/on/demandware.static/-/Sites-app_vinfast_vn-Library/default/dw1a73c862/images/PDP/vf9/202406/exterior/CE1W.webp", false, 438, "VF 9 Plus", "VinFast VF 9", "59I-66677", 1600000m, 200000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 7 },
                    { 17, 82m, "Tesla", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"autopilot\":true,\"7seats\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSQuPPGwtfobtphn2JnZfRLJU_ELJXj4mEweQ&s", false, 525, "Model Y Long Range", "Tesla Model Y", "51I-77788", 1900000m, 240000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 7 },
                    { 19, 84m, "Hyundai", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"solarRoof\":true}", "https://i1-vnexpress.vnecdn.net/2023/07/31/Hyundai-IONIQ-5-7.jpg?w=2400&h=0&q=100&dpr=1&fit=crop&s=gqOfVmNy6EZxHps0rNBfCA&t=image", false, 481, "Ioniq 5 Long Range", "Hyundai Ioniq 5", "59J-88899", 1500000m, 190000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 21, 84m, "Kia", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"fastCharging\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTDRNkoUruCHw69jdLnwmvz09ncLCsAcLnsJA&s", false, 528, "EV6 GT-Line", "Kia EV6", "51J-99900", 1550000m, 195000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 23, 100m, "Polestar", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"googleIntegration\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTFP_xRoDnlB2qUUnef4lw1c-HTf7Xnvi_hWw&s", false, 540, "Polestar 2 Long Range", "Polestar 2", "59L-33322", 1850000m, 230000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 25, 60.48m, "BYD", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"panoramicSunroof\":true}", "https://img1.oto.com.vn/2024/07/26/OpzfnMD2/atto-3-0f7e.webp", false, 480, "Atto 3 Extended", "BYD Atto 3", "51M-66655", 1450000m, 185000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 27, 87m, "Nissan", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"proPilot\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTNR4feHONvrN5Y-HK13689YKvLgkYgtqWiyA&s", false, 500, "Ariya e-4ORCE", "Nissan Ariya", "51L-44433", 1700000m, 210000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 28, 112m, "Lexus", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"markLevinson\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT8Xr-6nI1VqKU3Jn7pKYqIw-8L_xK3y8T9cxo&s", false, 565, "ES 300h Hybrid", "Lexus ES 300h", "59N-11234", 2000000m, 250000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 29, 78m, "Volvo", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"panoramicSunroof\":true,\"safetyFeatures\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQqJME5P0q8Z9p5Y5e5o5o5o5o5o5o5o5o&s", false, 470, "XC60 Recharge", "Volvo XC60", "51N-22345", 1900000m, 240000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 30, 100m, "Jaguar", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"premiumAudio\":true,\"fastCharging\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS3_3_3_3_3_3_3_3&s", false, 480, "I-PACE SE", "Jaguar I-PACE", "59O-33456", 2150000m, 270000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 31, 66m, "Chevrolet", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"backupCamera\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ4_4_4_4_4_4_4_4&s", false, 417, "Bolt EV", "Chevrolet Bolt EV", "51O-44567", 1280000m, 160000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 32, 82m, "Volkswagen", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"spacious\":true,\"familyFriendly\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR5_5_5_5_5_5_5_5&s", false, 450, "ID.Buzz Pro", "Volkswagen ID.Buzz", "59P-55678", 2200000m, 280000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 7 },
                    { 33, 112.5m, "Lucid", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"luxuryInterior\":true,\"advancedTech\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS6_6_6_6_6_6_6_6&s", false, 650, "Air Touring", "Lucid Air", "51P-66789", 3200000m, 400000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 34, 112m, "Fisker", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"solarPanel\":true,\"sustainableDesign\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS7_7_7_7_7_7_7_7&s", false, 440, "Ocean Extreme", "Fisker Ocean", "59Q-77890", 1600000m, 200000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 35, 99m, "Genesis", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"leatherSeats\":true,\"premiumAudio\":true,\"luxuryBrand\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS8_8_8_8_8_8_8_8&s", false, 480, "GV70 Electrified", "Genesis GV70", "51Q-88901", 2150000m, 270000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 36, 71.4m, "Subaru", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"awd\":true,\"offRoad\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS9_9_9_9_9_9_9_9&s", false, 460, "Solterra Premium", "Subaru Solterra", "59R-99012", 1750000m, 220000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 37, 112m, "Lotus", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"sportDesign\":true,\"performanceMode\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS0_0_0_0_0_0_0_0&s", false, 520, "Eletre Sport", "Lotus Eletre", "51R-00123", 2300000m, 290000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 38, 35.5m, "Mazda", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"stylishDesign\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS1_1_1_1_1_1_1_1&s", false, 290, "MX-30", "Mazda MX-30", "59S-11234", 1360000m, 170000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 39, 52m, "Renault", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"retroDesign\":true,\"sporty\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS2_2_2_2_2_2_2_2&s", false, 380, "5 Turbo 3E", "Renault 5 Turbo 3E", "51S-22345", 1440000m, 180000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 4 },
                    { 40, 42m, "Fiat", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"compact\":true,\"cityFriendly\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS3_3_3_3_3_3_3_3&s", false, 330, "500e", "Fiat 500e", "59T-33456", 1120000m, 140000m, 3, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 4 },
                    { 41, 54m, "MINI", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"compact\":true,\"fun\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS4_4_4_4_4_4_4_4&s", false, 270, "Cooper SE", "MINI Cooper SE", "51T-44567", 1240000m, 155000m, 4, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 4 },
                    { 42, 77m, "Opel", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"spacious\":true,\"practical\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS5_5_5_5_5_5_5_5&s", false, 440, "Grandland Electric", "Opel Grandland", "59U-55678", 1600000m, 200000m, 1, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 },
                    { 43, 82m, "Cupra", new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "{\"gps\":true,\"insurance\":true,\"bluetooth\":true,\"airConditioner\":true,\"sportStyle\":true,\"performance\":true}", "https://encrypted-tbn0.gstatic.com/images?q=tbn:AnD9GcS6_6_6_6_6_6_6_6&s", false, 500, "Born e-Boost", "Cupra Born", "51U-66789", 1680000m, 210000m, 2, 0, new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "car", 5 }
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
