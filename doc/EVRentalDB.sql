-- SQL Server script for EVRentalDB
-- Generated from C# Entity/Enum definitions
-- Updated: October 29, 2025

CREATE DATABASE [EVRentalDB];
GO
USE [EVRentalDB];
GO

-- =============================================
-- Table: Account
-- Description: Stores user account information (Renter, Staff, Admin)
-- =============================================
CREATE TABLE [Account] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Phone NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role INT NOT NULL, -- AccountRole enum (0: Renter, 1: Staff, 2: Admin)
    IsActive BIT NOT NULL DEFAULT 0, -- Account activation status
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- =============================================
-- Table: Station
-- Description: Stores information about rental stations
-- =============================================
CREATE TABLE [Station] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    State NVARCHAR(100) NOT NULL, -- State/Province/City
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- =============================================
-- Table: Vehicle
-- Description: Stores electric vehicle information
-- =============================================
CREATE TABLE [Vehicle] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StationId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL, -- Vehicle name (e.g., VinFast VF7 Plus 2025)
    Brand NVARCHAR(100) NOT NULL, -- Vehicle brand
    PlateNumber NVARCHAR(50) NOT NULL UNIQUE, -- License plate number
    Model NVARCHAR(100) NOT NULL,
    VehicleType NVARCHAR(50) NOT NULL, -- 'scooter'|'motorbike'|'car'
    Status INT NOT NULL DEFAULT 0, -- VehicleStatus enum (0: Available, 1: Rented, 2: Maintenance, 3: Charging)
    PricePerHour DECIMAL(18,2) NOT NULL,
    PricePerDay DECIMAL(18,2) NOT NULL,
    Features NVARCHAR(MAX) NULL, -- JSON: {"gps":true,"insurance":true}
    ImageUrl NVARCHAR(500) NULL,
    MaxDistance INT NOT NULL, -- Maximum distance in km
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Vehicle_Station FOREIGN KEY (StationId) REFERENCES [Station](Id) ON DELETE CASCADE
);

-- =============================================
-- Table: RentalRecord
-- Description: Stores rental transaction records
-- =============================================
CREATE TABLE [RentalRecord] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RenterId INT NOT NULL, -- References Account.Id
    VehicleId INT NOT NULL,
    PickupStationId INT NOT NULL,
    ReturnStationId INT NULL, -- Can be different from pickup station
    StartTime DATETIME NULL,
    ExpectedEndTime DATETIME NULL,
    ActualEndTime DATETIME NULL,
    Status INT NOT NULL DEFAULT 0, -- RentalRecordStatus enum (0: Pending, 1: Active, 2: Completed, 3: Cancelled)
    BasePrice DECIMAL(18,2) NOT NULL,
    DepositFee DECIMAL(18,2) NOT NULL, -- Security deposit
    ReservationFee DECIMAL(18,2) NOT NULL, -- Reservation fee
    ExtraFees DECIMAL(18,2) NOT NULL DEFAULT 0, -- Additional charges
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    TotalPrice DECIMAL(18,2) NOT NULL,
    OtpCode NVARCHAR(10) NULL, -- OTP code for vehicle pickup (6 characters)
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_RentalRecord_Renter FOREIGN KEY (RenterId) REFERENCES [Account](Id),
    CONSTRAINT FK_RentalRecord_Vehicle FOREIGN KEY (VehicleId) REFERENCES [Vehicle](Id),
    CONSTRAINT FK_RentalRecord_PickupStation FOREIGN KEY (PickupStationId) REFERENCES [Station](Id),
    CONSTRAINT FK_RentalRecord_ReturnStation FOREIGN KEY (ReturnStationId) REFERENCES [Station](Id)
);

-- =============================================
-- Table: Payment
-- Description: Stores payment information for rentals
-- =============================================
CREATE TABLE [Payment] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RentalId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL, -- Total payment amount
    Method NVARCHAR(50) NOT NULL, -- 'cash'|'card'|'e-wallet'|'bank-transfer'
    TransactionRef NVARCHAR(100) NULL, -- Payment gateway transaction reference
    Status NVARCHAR(50) NOT NULL DEFAULT 'pending', -- 'pending'|'paid'|'failed'|'refunded'
    PaidAt DATETIME NULL, -- Payment completion timestamp
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Payment_RentalRecord FOREIGN KEY (RentalId) REFERENCES [RentalRecord](Id) ON DELETE CASCADE
);

-- =============================================
-- Table: InspectionProblem
-- Description: Stores inspection issues and incidents
-- =============================================
CREATE TABLE [InspectionProblem] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RentalId INT NOT NULL,
    IncidentType NVARCHAR(50) NOT NULL, -- 'damage'|'late_return'|'no_show'|'nonpayment'|'other'
    Description NVARCHAR(MAX) NOT NULL,
    Evidence NVARCHAR(MAX) NULL, -- JSON array of image URLs, video references
    PenaltyAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedBy INT NULL, -- Staff/Admin who created the report
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_InspectionProblem_RentalRecord FOREIGN KEY (RentalId) REFERENCES [RentalRecord](Id) ON DELETE CASCADE
);

-- =============================================
-- Table: RatingReview
-- Description: Stores customer ratings and reviews
-- =============================================
CREATE TABLE [RatingReview] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RentalId INT NOT NULL UNIQUE, -- One review per rental
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5), -- Rating from 1 to 5
    Comment NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_RatingReview_RentalRecord FOREIGN KEY (RentalId) REFERENCES [RentalRecord](Id) ON DELETE CASCADE
);

-- =============================================
-- Indexes for better query performance
-- =============================================
CREATE INDEX IX_Account_Email ON [Account](Email);
CREATE INDEX IX_Account_Role ON [Account](Role);
CREATE INDEX IX_Vehicle_Status ON [Vehicle](Status);
CREATE INDEX IX_Vehicle_StationId ON [Vehicle](StationId);
CREATE INDEX IX_RentalRecord_RenterId ON [RentalRecord](RenterId);
CREATE INDEX IX_RentalRecord_VehicleId ON [RentalRecord](VehicleId);
CREATE INDEX IX_RentalRecord_Status ON [RentalRecord](Status);
CREATE INDEX IX_Payment_RentalId ON [Payment](RentalId);
CREATE INDEX IX_Payment_Status ON [Payment](Status);

-- End of script
