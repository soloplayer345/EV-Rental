-- =============================================
-- EV Rental DB - Sample Data
-- Generated: November 15, 2025
-- Description: Demo data for testing and development
-- =============================================

USE [EVRentalDB];
GO

-- =============================================
-- Clear existing data (optional - comment out if needed)
-- =============================================
-- DELETE FROM [RatingReview];
-- DELETE FROM [InspectionProblem];
-- DELETE FROM [Payment];
-- DELETE FROM [RentalRecord];
-- DELETE FROM [Vehicle];
-- DELETE FROM [Station];
-- DELETE FROM [Account];
-- GO

-- =============================================
-- Sample Data: Account (Users)
-- AccountRole: 0 = Renter, 1 = Staff, 2 = Admin
-- =============================================
INSERT INTO [Account] (FullName, Email, Phone, PasswordHash, Role, IsActive, CreatedAt)
VALUES 
-- Admin accounts
('Nguyễn Văn Admin', 'admin@evrentals.com', '0901234567', 'hashed_password_admin', 2, 1, GETDATE()),
-- Staff accounts
('Trần Thị Staff', 'staff.tran@evrentals.com', '0902345678', 'hashed_password_staff1', 1, 1, GETDATE()),
('Lê Minh Staff', 'staff.minh@evrentals.com', '0903456789', 'hashed_password_staff2', 1, 1, GETDATE()),
-- Renter accounts
('Phạm Quốc Hùng', 'hung.pham@gmail.com', '0904567890', 'hashed_password_renter1', 0, 1, GETDATE()),
('Vũ Thị Trang', 'trang.vu@gmail.com', '0905678901', 'hashed_password_renter2', 0, 1, GETDATE()),
('Đặng Hữu Trí', 'tri.dang@gmail.com', '0906789012', 'hashed_password_renter3', 0, 1, GETDATE()),
('Bùi Minh Châu', 'chau.bui@gmail.com', '0907890123', 'hashed_password_renter4', 0, 1, GETDATE()),
('Hoàng Văn Tú', 'tu.hoang@gmail.com', '0908901234', 'hashed_password_renter5', 0, 1, GETDATE()),
('Ngô Thị Liên', 'lien.ngo@gmail.com', '0909012345', 'hashed_password_renter6', 0, 0, GETDATE());
GO

-- =============================================
-- Sample Data: Station (Rental Stations)
-- =============================================
INSERT INTO [Station] (Name, Address, State, CreatedAt)
VALUES 
('Trạm Thảo Cầm Viên', '1 Nguyễn Bỉnh Khiêm, Phường Đa Kao, Quận 1', 'TP. Hồ Chí Minh', GETDATE()),
('Trạm Bến Thành', '58 Tôn Thất Thuyết, Phường Bến Thành, Quận 1', 'TP. Hồ Chí Minh', GETDATE()),
('Trạm Hàng Xanh', '273 Nguyễn Huệ, Phường Bến Thành, Quận 1', 'TP. Hồ Chí Minh', GETDATE()),
('Trạm Tạo Đàn', '119 Trần Hưng Đạo, Phường Cầu Ông Lãnh, Quận 1', 'TP. Hồ Chí Minh', GETDATE()),
('Trạm Tân Bình', '1000 Cách Mạng Tháng 8, Phường 4, Quận Tân Bình', 'TP. Hồ Chí Minh', GETDATE()),
('Trạm Biên Hòa', '42 Đường 30/4, Phường Trung Dũng, TP Biên Hòa', 'Đồng Nai', GETDATE());
GO

-- =============================================
-- Sample Data: Vehicle (Electric Vehicles)
-- VehicleStatus: 0 = Available, 1 = Rented, 2 = Maintenance, 3 = Charging
-- =============================================
INSERT INTO [Vehicle] (StationId, Name, Brand, PlateNumber, Model, VehicleType, Status, PricePerHour, PricePerDay, Features, ImageUrl, MaxDistance, SeatCapacity, BatteryCapacity, CreatedAt)
VALUES 
-- VinFast vehicles
(1, 'VinFast VF7 Plus 2025', 'VinFast', '51A12345', 'VF7 Plus', 'car', 0, 150000, 800000, '{"gps":true,"insurance":true,"wifi":true}', 'https://example.com/vf7.jpg', 400, 5, 75.0, GETDATE()),
(1, 'VinFast VF5 Plus 2025', 'VinFast', '51A12346', 'VF5 Plus', 'car', 0, 120000, 600000, '{"gps":true,"insurance":true}', 'https://example.com/vf5.jpg', 350, 5, 55.0, GETDATE()),
(2, 'VinFast VF8 SUV 2025', 'VinFast', '51B12347', 'VF8 SUV', 'car', 0, 180000, 950000, '{"gps":true,"insurance":true,"wifi":true,"phone_charging":true}', 'https://example.com/vf8.jpg', 450, 7, 85.0, GETDATE()),
-- Tesla vehicles
(2, 'Tesla Model 3 Standard', 'Tesla', '51C12348', 'Model 3', 'car', 1, 200000, 1100000, '{"gps":true,"insurance":true,"autopilot":true}', 'https://example.com/tesla3.jpg', 500, 5, 60.0, GETDATE()),
(3, 'Tesla Model Y Long Range', 'Tesla', '51D12349', 'Model Y', 'car', 0, 220000, 1300000, '{"gps":true,"insurance":true,"autopilot":true,"panoramic_roof":true}', 'https://example.com/teslay.jpg', 550, 5, 75.0, GETDATE()),
-- BMW vehicles
(3, 'BMW i3 eDrive', 'BMW', '51E12350', 'i3 eDrive', 'car', 2, 160000, 850000, '{"gps":true,"insurance":true}', 'https://example.com/bmwi3.jpg', 380, 4, 42.0, GETDATE()),
-- Scooter and Motorbike
(4, 'VinFast e-Scooter Pro', 'VinFast', '51F12351', 'e-Scooter', 'scooter', 0, 25000, 150000, '{"gps":true}', 'https://example.com/scooter.jpg', 80, 1, 10.0, GETDATE()),
(4, 'VinFast e-Motorbike', 'VinFast', '51G12352', 'e-Motorbike', 'motorbike', 0, 50000, 300000, '{"gps":true,"insurance":true}', 'https://example.com/motorbike.jpg', 200, 2, 20.0, GETDATE()),
(5, 'VinFast VF7 Plus 2025', 'VinFast', '51H12353', 'VF7 Plus', 'car', 3, 150000, 800000, '{"gps":true,"insurance":true,"wifi":true}', 'https://example.com/vf7-2.jpg', 400, 5, 75.0, GETDATE()),
(5, 'Tesla Model Y Standard', 'Tesla', '51I12354', 'Model Y', 'car', 0, 200000, 1200000, '{"gps":true,"insurance":true,"autopilot":true}', 'https://example.com/teslay-2.jpg', 520, 5, 70.0, GETDATE()),
(6, 'VinFast VF5 Plus 2025', 'VinFast', '51J12355', 'VF5 Plus', 'car', 0, 120000, 600000, '{"gps":true,"insurance":true}', 'https://example.com/vf5-2.jpg', 350, 5, 55.0, GETDATE());
GO

-- =============================================
-- Sample Data: RentalRecord (Rental Transactions)
-- RentalRecordStatus: 0 = Pending, 1 = Active, 2 = Completed, 3 = Cancelled
-- =============================================
INSERT INTO [RentalRecord] (RenterId, VehicleId, PickupStationId, ReturnStationId, StartTime, ExpectedEndTime, ActualEndTime, Status, BasePrice, DepositFee, ReservationFee, ExtraFees, Discount, TotalPrice, OtpCode, CreatedAt)
VALUES 
-- Pending rental
(4, 1, 1, 1, NULL, DATEADD(DAY, 2, GETDATE()), NULL, 0, 1600000, 200000, 50000, 0, 0, 1850000, '123456', GETDATE()),
-- Active rental
(5, 2, 2, 2, GETDATE(), DATEADD(DAY, 1, GETDATE()), NULL, 1, 600000, 150000, 30000, 0, 0, 780000, '654321', GETDATE()),
-- Completed rentals
(6, 3, 3, 1, DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, -3, GETDATE()), 2, 950000, 250000, 50000, 100000, 50000, 1300000, '789012', DATEADD(DAY, -5, GETDATE())),
(7, 5, 2, 3, DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -8, GETDATE()), DATEADD(DAY, -8, GETDATE()), 2, 1300000, 300000, 100000, 0, 100000, 1600000, '345678', DATEADD(DAY, -10, GETDATE())),
(8, 7, 4, 4, DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, -1, GETDATE()), 2, 150000, 50000, 10000, 0, 0, 210000, '901234', DATEADD(DAY, -2, GETDATE())),
-- Cancelled rental
(4, 8, 4, 4, NULL, DATEADD(DAY, 3, GETDATE()), NULL, 3, 300000, 100000, 20000, 0, 0, 420000, '567890', DATEADD(DAY, -1, GETDATE()));
GO

-- =============================================
-- Sample Data: Payment (Payment Records)
-- PaymentStatus: 'pending'|'paid'|'failed'|'refunded'
-- PaymentMethod: 'cash'|'card'|'e-wallet'|'bank-transfer'
-- =============================================
INSERT INTO [Payment] (RentalId, Amount, Method, TransactionRef, Status, PaidAt, CreatedAt)
VALUES 
-- Payments for rentals
(1, 1850000, 'card', 'TXN20251115001', 'paid', GETDATE(), GETDATE()),
(2, 780000, 'e-wallet', 'TXN20251115002', 'paid', GETDATE(), GETDATE()),
(3, 1300000, 'bank-transfer', 'TXN20251110001', 'paid', DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, -5, GETDATE())),
(4, 1600000, 'card', 'TXN20251105001', 'paid', DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -10, GETDATE())),
(5, 210000, 'cash', 'CASH20251113001', 'paid', DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, -2, GETDATE())),
(6, 0, 'card', 'TXN20251114001', 'refunded', DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, -1, GETDATE()));
GO

-- =============================================
-- Sample Data: InspectionProblem (Issues/Damage Reports)
-- IncidentType: 'damage'|'late_return'|'no_show'|'nonpayment'|'other'
-- =============================================
INSERT INTO [InspectionProblem] (RentalId, IncidentType, Description, Evidence, PenaltyAmount, CreatedBy, CreatedAt)
VALUES 
(3, 'damage', 'Vết xước trên cánh cửa trước bên trái', '["https://example.com/damage1.jpg","https://example.com/damage2.jpg"]', 500000, 2, GETDATE()),
(4, 'late_return', 'Trả xe muộn 2 giờ', '["https://example.com/receipt.jpg"]', 100000, 3, GETDATE()),
(6, 'no_show', 'Không xuất hiện để nhận xe', NULL, 200000, 2, GETDATE());
GO

-- =============================================
-- Sample Data: RatingReview (Customer Reviews)
-- Rating: 1-5 stars
-- =============================================
INSERT INTO [RatingReview] (RentalId, Rating, Comment, CreatedAt)
VALUES 
(3, 5, 'Xe mới, sạch sẽ, dịch vụ tuyệt vời! Sẽ thuê lại.', DATEADD(DAY, -5, GETDATE())),
(4, 4, 'Tổng thể tốt, nhân viên thân thiện. Chỉ cần cải thiện thời gian check-in.', DATEADD(DAY, -10, GETDATE())),
(5, 3, 'Scooter hoạt động bình thường nhưng pin hao nhanh hơn dự kiến.', DATEADD(DAY, -2, GETDATE()));
GO

-- =============================================
-- Verify inserted data
-- =============================================
SELECT 'Account Records' AS TableName, COUNT(*) AS RecordCount FROM [Account]
UNION ALL
SELECT 'Station Records', COUNT(*) FROM [Station]
UNION ALL
SELECT 'Vehicle Records', COUNT(*) FROM [Vehicle]
UNION ALL
SELECT 'Rental Records', COUNT(*) FROM [RentalRecord]
UNION ALL
SELECT 'Payment Records', COUNT(*) FROM [Payment]
UNION ALL
SELECT 'Inspection Problems', COUNT(*) FROM [InspectionProblem]
UNION ALL
SELECT 'Ratings & Reviews', COUNT(*) FROM [RatingReview];
GO

-- =============================================
-- Sample Query: View rental summary
-- =============================================
-- SELECT 
--     r.Id,
--     ac.FullName,
--     v.Name,
--     s.Name AS PickupStation,
--     r.StartTime,
--     r.ActualEndTime,
--     r.TotalPrice,
--     r.Status
-- FROM [RentalRecord] r
-- INNER JOIN [Account] ac ON r.RenterId = ac.Id
-- INNER JOIN [Vehicle] v ON r.VehicleId = v.Id
-- INNER JOIN [Station] s ON r.PickupStationId = s.Id
-- ORDER BY r.CreatedAt DESC;
