using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class RentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly VehicleService _vehicleService;

        public RentalService(IUnitOfWork unitOfWork, VehicleService vehicleService)
        {
            _unitOfWork = unitOfWork;
            _vehicleService = vehicleService;
        }

        public async Task<List<StationDto>> GetAllStationsAsync()
        {
            var stationRepo = _unitOfWork.GetRepository<Station>();
            var stations = await stationRepo.GetAllAsync();
            
            return stations.Select(s => new StationDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                State = s.State
            }).ToList();
        }

        public async Task<ServiceResultDto<PendingBookingDto>> CalculateBookingCostAsync(int accountId, CreateRentalRequestDto request)
        {
            try
            {
                // Validate dates
                if (request.StartTime < DateTime.Now)
                {
                    return ServiceResultDto<PendingBookingDto>.FailureResult("Thời gian bắt đầu phải lớn hơn thời gian hiện tại.");
                }

                if (request.ExpectedEndTime <= request.StartTime)
                {
                    return ServiceResultDto<PendingBookingDto>.FailureResult("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
                }

                // Get vehicle
                var vehicle = await _vehicleService.GetVehicleByIdAsync(request.VehicleId);
                if (vehicle == null)
                {
                    return ServiceResultDto<PendingBookingDto>.FailureResult("Không tìm thấy xe.");
                }

                if (vehicle.Status != VehicleStatus.Available)
                {
                    return ServiceResultDto<PendingBookingDto>.FailureResult("Xe này hiện không khả dụng để thuê.");
                }

                // Calculate total cost
                TimeSpan duration = request.ExpectedEndTime - request.StartTime;
                decimal basePrice;

                if (duration.TotalDays >= 1)
                {
                    int days = (int)Math.Ceiling(duration.TotalDays);
                    basePrice = days * vehicle.PricePerDay;
                }
                else
                {
                    int hours = (int)Math.Ceiling(duration.TotalHours);
                    basePrice = hours * vehicle.PricePerHour;
                }

                // Calculate additional fees
                TimeSpan untilPickup = request.StartTime - DateTime.Now;
                int daysUntilPickup = (int)Math.Ceiling(untilPickup.TotalDays);
                
                // Reservation fee (if booking more than 1 day in advance)
                decimal reservationFee = 0;
                if (daysUntilPickup > 1)
                {
                    reservationFee = Math.Max(basePrice * 0.1m, 50000);
                }

                // Deposit fee (fixed)
                decimal depositFee = 3000000;

                // Total cost
                decimal totalCost = basePrice + reservationFee + depositFee;

                // Create pending booking DTO
                var pendingBooking = new PendingBookingDto
                {
                    AccountId = accountId,
                    VehicleId = request.VehicleId,
                    PickupStationId = request.PickupStationId,
                    ReturnStationId = request.ReturnStationId,
                    StartTime = request.StartTime,
                    ExpectedEndTime = request.ExpectedEndTime,
                    BasePrice = basePrice,
                    ReservationFee = reservationFee,
                    DepositFee = depositFee,
                    TotalPrice = totalCost,
                    Notes = request.Notes
                };

                return ServiceResultDto<PendingBookingDto>.SuccessResult(pendingBooking, "Tính toán chi phí thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<PendingBookingDto>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResultDto<RentalRecord>> CreatePendingRentalAsync(PendingBookingDto bookingDto, string otpCode)
        {
            try
            {
                // Validate vehicle is still available
                var vehicle = await _vehicleService.GetVehicleByIdAsync(bookingDto.VehicleId);
                if (vehicle == null)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Không tìm thấy xe.");
                }

                if (vehicle.Status != VehicleStatus.Available)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Xe này hiện không khả dụng để thuê.");
                }

                // Create rental record with Pending status
                var rentalRecord = new RentalRecord
                {
                    RenterId = bookingDto.AccountId,
                    VehicleId = bookingDto.VehicleId,
                    PickupStationId = bookingDto.PickupStationId,
                    ReturnStationId = bookingDto.ReturnStationId,
                    StartTime = bookingDto.StartTime,
                    ExpectedEndTime = bookingDto.ExpectedEndTime,
                    BasePrice = bookingDto.BasePrice,
                    ReservationFee = bookingDto.ReservationFee,
                    DepositFee = bookingDto.DepositFee,
                    TotalPrice = bookingDto.TotalPrice,
                    Status = RentalRecordStatus.Pending,
                    OtpCode = otpCode, // Use OTP as identifier
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    IsDeleted = false
                };

                // Save rental record
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                await rentalRepo.AddAsync(rentalRecord);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResultDto<RentalRecord>.SuccessResult(rentalRecord, "Tạo đơn thuê xe (chờ thanh toán) thành công!");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<RentalRecord>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResultDto<RentalRecord>> CreateRentalAfterPaymentAsync(PendingBookingDto bookingDto)
        {
            try
            {
                // Validate vehicle is still available
                var vehicle = await _vehicleService.GetVehicleByIdAsync(bookingDto.VehicleId);
                if (vehicle == null)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Không tìm thấy xe.");
                }

                if (vehicle.Status != VehicleStatus.Available)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Xe này hiện không khả dụng để thuê.");
                }

                // Create rental record
                var rentalRecord = new RentalRecord
                {
                    RenterId = bookingDto.AccountId,
                    VehicleId = bookingDto.VehicleId,
                    PickupStationId = bookingDto.PickupStationId,
                    ReturnStationId = bookingDto.ReturnStationId,
                    StartTime = bookingDto.StartTime,
                    ExpectedEndTime = bookingDto.ExpectedEndTime,
                    BasePrice = bookingDto.BasePrice,
                    ReservationFee = bookingDto.ReservationFee,
                    DepositFee = bookingDto.DepositFee,
                    TotalPrice = bookingDto.TotalPrice,
                    Status = RentalRecordStatus.Confirmed,
                    OtpCode = GenerateOtpCode(),
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    IsDeleted = false
                };

                // Save rental record
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                await rentalRepo.AddAsync(rentalRecord);

                // Update vehicle status to Rented
                vehicle.Status = VehicleStatus.Rented;
                await _vehicleService.UpdateVehicleAsync(vehicle);

                await _unitOfWork.SaveChangesAsync();

                return ServiceResultDto<RentalRecord>.SuccessResult(rentalRecord, "Tạo đơn thuê xe thành công!");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<RentalRecord>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResultDto<RentalRecord>> ConfirmPendingRentalAsync(int rentalId)
        {
            try
            {
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                var rental = await GetRentalByIdAsync(rentalId);
                
                if (rental == null)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Không tìm thấy đơn thuê xe.");
                }

                if (rental.Status != RentalRecordStatus.Pending)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Đơn thuê xe không ở trạng thái chờ thanh toán.");
                }

                // Update rental status to Confirmed
                rental.Status = RentalRecordStatus.Confirmed;
                rental.UpdateDate = DateTime.Now;
                await rentalRepo.Update(rental);

                // Update vehicle status to Rented
                var vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Status = VehicleStatus.Rented;
                    await _vehicleService.UpdateVehicleAsync(vehicle);
                }

                await _unitOfWork.SaveChangesAsync();

                return ServiceResultDto<RentalRecord>.SuccessResult(rental, "Xác nhận đơn thuê xe thành công!");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<RentalRecord>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<RentalRecord?> GetPendingRentalByAccountAndVehicleAsync(int accountId, int vehicleId, DateTime startTime)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var rentals = await rentalRepo.GetAllAsync();
            
            // Find the most recent pending rental for this account, vehicle and start time
            return rentals
                .Where(r => r.RenterId == accountId 
                    && r.VehicleId == vehicleId 
                    && r.StartTime == startTime
                    && r.Status == RentalRecordStatus.Pending 
                    && !r.IsDeleted)
                .OrderByDescending(r => r.CreateDate)
                .FirstOrDefault();
        }

        private string GenerateOtpCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<List<RentalRecord>> GetRentalsByRenterIdAsync(int renterId)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var rentals = await rentalRepo.GetAllAsync();
            
            return rentals
                .Where(r => r.RenterId == renterId && !r.IsDeleted)
                .OrderByDescending(r => r.CreateDate)
                .ToList();
        }

        public async Task<RentalRecord?> GetRentalByIdAsync(int rentalId)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var rentals = await rentalRepo.GetAllAsync();
            return rentals.FirstOrDefault(r => r.Id == rentalId && !r.IsDeleted);
        }

        public async Task<ServiceResultDto<object>> CancelRentalAsync(int rentalId, int renterId)
        {
            try
            {
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                var rental = await GetRentalByIdAsync(rentalId);

                if (rental == null)
                {
                    return ServiceResultDto<object>.FailureResult("Không tìm thấy đơn thuê.");
                }

                // Kiểm tra quyền hủy đơn
                if (rental.RenterId != renterId)
                {
                    return ServiceResultDto<object>.FailureResult("Bạn không có quyền hủy đơn này.");
                }

                // Chỉ cho phép hủy đơn có trạng thái Pending hoặc Confirmed
                if (rental.Status != RentalRecordStatus.Pending && rental.Status != RentalRecordStatus.Confirmed)
                {
                    return ServiceResultDto<object>.FailureResult("Chỉ có thể hủy đơn ở trạng thái Chờ Thanh Toán hoặc Đã Xác Nhận.");
                }

                // Cập nhật trạng thái đơn thuê
                rental.Status = RentalRecordStatus.Cancelled;
                rental.UpdateDate = DateTime.Now;
                await rentalRepo.Update(rental);

                // Cập nhật trạng thái xe về Available
                var vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Status = VehicleStatus.Available;
                    await _vehicleService.UpdateVehicleAsync(vehicle);
                }

                await _unitOfWork.SaveChangesAsync();

                return ServiceResultDto<object>.SuccessResult("success", "Hủy đơn thuê thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<object>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        /// <summary>
        /// Tìm đơn thuê theo mã OTP
        /// </summary>
        public async Task<RentalRecord?> GetRentalByOtpCodeAsync(string otpCode)
        {
            if (string.IsNullOrWhiteSpace(otpCode))
                return null;

            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var query = rentalRepo.GetAllQueryable("Vehicle,Renter,PickupStation,ReturnStation");
            
            return await query
                .FirstOrDefaultAsync(r => r.OtpCode == otpCode 
                    && r.Status == RentalRecordStatus.Confirmed 
                    && !r.IsDeleted);
        }

        /// <summary>
        /// Xác minh OTP và cập nhật trạng thái đơn thuê khi khách nhận xe
        /// Cập nhật status từ Confirmed → Active và StartTime = DateTime.Now
        /// </summary>
        public async Task<ServiceResultDto<RentalRecord>> VerifyOtpAndPickupVehicleAsync(string otpCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(otpCode))
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Mã OTP không được để trống.");
                }

                // Tìm đơn thuê theo OTP
                var rental = await GetRentalByOtpCodeAsync(otpCode);
                
                if (rental == null)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Mã OTP không hợp lệ hoặc đơn thuê không tồn tại.");
                }

                if (rental.Status != RentalRecordStatus.Confirmed)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult($"Đơn thuê không ở trạng thái chờ nhận xe. Trạng thái hiện tại: {rental.Status}");
                }

                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();

                // Cập nhật trạng thái từ Confirmed → Active
                rental.Status = RentalRecordStatus.Active;
                rental.StartTime = DateTime.Now; 
                rental.UpdateDate = DateTime.Now;
                await rentalRepo.Update(rental);

                await _unitOfWork.SaveChangesAsync();

                return ServiceResultDto<RentalRecord>.SuccessResult(rental, "Xác nhận nhận xe thành công!");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<RentalRecord>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách đơn thuê chờ nhận tại trạm (status = Confirmed)
        /// </summary>
        public async Task<List<RentalRecord>> GetConfirmedRentalsByStationAsync(int stationId)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var query = rentalRepo.GetAllQueryable("Vehicle,Renter,PickupStation,ReturnStation");
            
            return await query
                .Where(r => r.PickupStationId == stationId 
                    && r.Status == RentalRecordStatus.Confirmed 
                    && !r.IsDeleted)
                .OrderByDescending(r => r.CreateDate)
                .ToListAsync();
        }
    }
}
