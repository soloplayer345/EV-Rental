-- SQL Server script for EVRentalDB
-- Generated from C# Entity/Enum definitions

CREATE DATABASE [EVRentalDB];
GO
USE [EVRentalDB];
GO

-- Table: Account
CREATE TABLE [Account] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role INT NOT NULL, -- AccountRole enum (0: Renter, 1: Staff, 2: Admin)
    Status INT NOT NULL -- AccountStatus enum (0: Active, 1: Inactive, 2: Banned)
);

-- Table: Station
CREATE TABLE [Station] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL
);

-- Table: Staff
CREATE TABLE [Staff] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    StationId INT NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_Staff_Account FOREIGN KEY (AccountId) REFERENCES [Account](Id),
    CONSTRAINT FK_Staff_Station FOREIGN KEY (StationId) REFERENCES [Station](Id)
);

-- Table: Renter
CREATE TABLE [Renter] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AccountId INT NOT NULL,
    DriverLicense NVARCHAR(50) NOT NULL,
    IdCard NVARCHAR(50) NOT NULL,
    Status INT NOT NULL, -- RenterStatus enum (0: Pending, 1: Approved, 2: Rejected)
    ApprovedBy INT NULL,
    ApprovedAt DATETIME NULL,
    CONSTRAINT FK_Renter_Account FOREIGN KEY (AccountId) REFERENCES [Account](Id)
);

-- Table: Vehicle
CREATE TABLE [Vehicle] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LicensePlate NVARCHAR(50) NOT NULL,
    BatteryCapacity FLOAT NOT NULL,
    StationId INT NOT NULL,
    Status INT NOT NULL, -- VehicleStatus enum (0: Available, 1: Rented, 2: Maintenance)
    PricePerHour DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Vehicle_Station FOREIGN KEY (StationId) REFERENCES [Station](Id)
);

-- Table: RentalRecord
CREATE TABLE [RentalRecord] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RenterId INT NOT NULL,
    VehicleId INT NOT NULL,
    StationId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NULL,
    TotalCost DECIMAL(18,2) NOT NULL,
    Status INT NOT NULL, -- RentalRecordStatus enum (0: Pending, 1: Active, 2: Completed)
    ContractUrl NVARCHAR(255) NOT NULL,
    CONSTRAINT FK_RentalRecord_Renter FOREIGN KEY (RenterId) REFERENCES [Renter](Id),
    CONSTRAINT FK_RentalRecord_Vehicle FOREIGN KEY (VehicleId) REFERENCES [Vehicle](Id),
    CONSTRAINT FK_RentalRecord_Station FOREIGN KEY (StationId) REFERENCES [Station](Id)
);

-- Table: Payment
CREATE TABLE [Payment] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RentalId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Method NVARCHAR(50) NOT NULL,
    PaymentDate DATETIME NOT NULL,
    Deposit DECIMAL(18,2) NOT NULL,
    Refund DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Payment_RentalRecord FOREIGN KEY (RentalId) REFERENCES [RentalRecord](Id)
);

-- Table: VehicleCheck
CREATE TABLE [VehicleCheck] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RentalId INT NOT NULL,
    StaffId INT NOT NULL,
    CheckType INT NOT NULL, -- VehicleCheckType enum (0: CheckOut, 1: CheckIn)
    ConditionNote NVARCHAR(255) NOT NULL,
    PhotoUrl NVARCHAR(255) NOT NULL,
    Timestamp DATETIME NOT NULL,
    CONSTRAINT FK_VehicleCheck_RentalRecord FOREIGN KEY (RentalId) REFERENCES [RentalRecord](Id),
    CONSTRAINT FK_VehicleCheck_Staff FOREIGN KEY (StaffId) REFERENCES [Staff](Id)
);

-- Table: Report
CREATE TABLE [Report] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StationId INT NOT NULL,
    CreatedBy INT NOT NULL,
    ReportType NVARCHAR(100) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    CONSTRAINT FK_Report_Station FOREIGN KEY (StationId) REFERENCES [Station](Id)
);

-- Unique constraints and indexes (if needed)
-- ALTER TABLE ... ADD CONSTRAINT ...

-- End of script
