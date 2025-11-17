using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVehicleService _vehicleService;

        public RentalService(IUnitOfWork unitOfWork, IVehicleService vehicleService)
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
                DateTime minimumStartTime = DateTime.Now.AddHours(1);
                if (request.StartTime < minimumStartTime)
                {
                    return ServiceResultDto<PendingBookingDto>.FailureResult("Thời gian bắt đầu phải cách thời điểm hiện tại ít nhất 1 giờ.");
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

                // Update vehicle status to WaitingForPickup until OTP verification
                vehicle.Status = VehicleStatus.WaitingForPickup;
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

                // Update vehicle status to WaitingForPickup until OTP verification
                var vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Status = VehicleStatus.WaitingForPickup;
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

                var vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);
                if (vehicle == null)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Không tìm thấy thông tin xe tương ứng với đơn thuê.");
                }

                if (vehicle.Status != VehicleStatus.WaitingForPickup)
                {
                    return ServiceResultDto<RentalRecord>.FailureResult("Xe không ở trạng thái chờ nhận để bàn giao cho khách.");
                }

                // Cập nhật trạng thái từ Confirmed → Active
                rental.Status = RentalRecordStatus.Active;
                rental.StartTime = DateTime.Now; 
                rental.UpdateDate = DateTime.Now;
                await rentalRepo.Update(rental);

                // Sau khi khách đã nhận xe, cập nhật trạng thái xe sang Rented (đang được thuê)
                vehicle.Status = VehicleStatus.Rented;
                await _vehicleService.UpdateVehicleAsync(vehicle);

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

        /// <summary>
        /// Lấy thông tin chính sách hủy đơn và tính số tiền hoàn lại
        /// </summary>
        public async Task<ServiceResultDto<CancellationPolicyDto>> GetCancellationPolicyAsync(int rentalId, int renterId)
        {
            try
            {
                // 1. Lấy thông tin đơn thuê
                var rental = await GetRentalByIdAsync(rentalId);
                if (rental == null)
                    return ServiceResultDto<CancellationPolicyDto>.FailureResult("Không tìm thấy đơn thuê.");
                
                // 2. Kiểm tra quyền truy cập
                if (rental.RenterId != renterId)
                    return ServiceResultDto<CancellationPolicyDto>.FailureResult("Bạn không có quyền truy cập đơn này.");
                
                // 3. Kiểm tra trạng thái
                if (rental.Status != RentalRecordStatus.Pending && 
                    rental.Status != RentalRecordStatus.Confirmed)
                {
                    return ServiceResultDto<CancellationPolicyDto>.FailureResult(
                        "Chỉ có thể hủy đơn ở trạng thái Chờ Thanh Toán hoặc Đã Xác Nhận.");
                }
                
                // 4. Tính thời gian còn lại đến StartTime
                var timeUntilStart = rental.StartTime.Value - DateTime.Now;
                
                // 5. Áp dụng chính sách hoàn tiền
                decimal refundAmount = 0;
                string policyText = "";
                string refundPercentage = "";
                bool canCancel = false;
                
                if (timeUntilStart.TotalHours < 0)
                {
                    // Đã quá giờ StartTime → Không cho hủy
                    policyText = "Không thể hủy đơn đã quá thời gian bắt đầu.";
                    refundPercentage = "0%";
                    canCancel = false;
                }
                else if (timeUntilStart.TotalHours >= 24)
                {
                    // Hủy trước 24h → Hoàn 100%
                    refundAmount = rental.BasePrice;
                    policyText = "Hoàn 100% tiền thuê cơ bản vì hủy trước 24 giờ.";
                    refundPercentage = "100%";
                    canCancel = true;
                }
                else if (timeUntilStart.TotalHours >= 12)
                {
                    // Hủy từ 12-24h → Hoàn 50%
                    refundAmount = rental.BasePrice * 0.5m;
                    policyText = "Hoàn 50% tiền thuê cơ bản (phí hủy 50%) vì hủy trong khoảng 12-24 giờ trước.";
                    refundPercentage = "50%";
                    canCancel = true;
                }
                else
                {
                    // Hủy dưới 12h → Không hoàn
                    refundAmount = 0;
                    policyText = "Không hoàn tiền thuê vì hủy quá gần giờ nhận xe (dưới 12 giờ).";
                    refundPercentage = "0%";
                    canCancel = true;
                }
                
                // 6. Tạo DTO
                var policyDto = new CancellationPolicyDto
                {
                    RentalId = rental.Id,
                    CanCancel = canCancel,
                    RefundAmount = refundAmount,
                    BasePrice = rental.BasePrice,
                    ReservationFee = rental.ReservationFee,
                    DepositFee = rental.DepositFee,
                    PolicyText = policyText,
                    RefundPercentage = refundPercentage,
                    StartTime = rental.StartTime.Value,
                    TimeUntilStart = timeUntilStart
                };
                
                return ServiceResultDto<CancellationPolicyDto>.SuccessResult(
                    policyDto, "Lấy thông tin chính sách thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<CancellationPolicyDto>.FailureResult(
                    $"Có lỗi xảy ra: {ex.Message}");
            }
        }

        /// <summary>
        /// Hủy đơn thuê với chính sách hoàn tiền
        /// </summary>
        public async Task<ServiceResultDto<object>> CancelRentalWithPolicyAsync(int rentalId, int renterId, bool agreedToPolicy)
        {
            try
            {
                // 1. Validate user đồng ý chính sách
                if (!agreedToPolicy)
                {
                    return ServiceResultDto<object>.FailureResult(
                        "Bạn phải đồng ý với chính sách hủy đơn.");
                }
                
                // 2. Lấy thông tin đơn thuê
                var rental = await GetRentalByIdAsync(rentalId);
                if (rental == null)
                {
                    return ServiceResultDto<object>.FailureResult(
                        "Không tìm thấy đơn thuê.");
                }
                
                // 3. Validate quyền hủy đơn
                if (rental.RenterId != renterId)
                {
                    return ServiceResultDto<object>.FailureResult(
                        "Bạn không có quyền hủy đơn này.");
                }
                
                // 4. Validate status
                if (rental.Status != RentalRecordStatus.Pending && 
                    rental.Status != RentalRecordStatus.Confirmed)
                {
                    return ServiceResultDto<object>.FailureResult(
                        $"Chỉ có thể hủy đơn ở trạng thái Chờ Thanh Toán hoặc Đã Xác Nhận. " +
                        $"Trạng thái hiện tại: {rental.Status}");
                }
                
                // 5. Validate thời gian (không quá StartTime)
                if (rental.StartTime < DateTime.Now)
                {
                    return ServiceResultDto<object>.FailureResult(
                        "Không thể hủy đơn đã quá thời gian bắt đầu.");
                }
                
                // 6. Tính tiền hoàn lại
                var policyResult = await GetCancellationPolicyAsync(rentalId, renterId);
                if (!policyResult.Success)
                {
                    return ServiceResultDto<object>.FailureResult(policyResult.Message);
                }
                
                var policy = policyResult.Data;
                if (!policy.CanCancel)
                {
                    return ServiceResultDto<object>.FailureResult(
                        "Không thể hủy đơn thuê này theo chính sách.");
                }
                
                // 7. Lưu status cũ để kiểm tra đã thanh toán chưa
                var originalStatus = rental.Status;
                
                // 8. Cập nhật RentalRecord
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                rental.Status = RentalRecordStatus.Cancelled;
                rental.UpdateDate = DateTime.Now;
                await rentalRepo.Update(rental);
                
                // 9. Tạo refund record CHỈ KHI đơn đã được thanh toán (Confirmed)
                // Nếu đơn còn Pending (chưa thanh toán) thì không cần tạo refund
                if (originalStatus == RentalRecordStatus.Confirmed && policy.RefundAmount > 0)
                {
                    var paymentService = new PaymentService(_unitOfWork);
                    var refundResult = await paymentService.CreateRefundRecordAsync(rentalId, policy.RefundAmount);
                    
                    if (!refundResult.Success)
                    {
                        // Log error nhưng vẫn tiếp tục (có thể xử lý manual)
                        Console.WriteLine($"[WARNING] Failed to create refund record: {refundResult.Message}");
                    }
                }
                
                // 10. Cập nhật Vehicle
                var vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Status = VehicleStatus.Available;
                    await _vehicleService.UpdateVehicleAsync(vehicle);
                }
                
                // 11. Commit transaction
                await _unitOfWork.SaveChangesAsync();
                
                // 12. Return success với thông báo phù hợp
                string successMessage;
                if (originalStatus == RentalRecordStatus.Pending)
                {
                    // Chưa thanh toán -> Không có refund
                    successMessage = "Hủy đơn thành công. Đơn thuê chưa được thanh toán nên không có khoản tiền nào được hoàn lại.";
                }
                else if (policy.RefundAmount > 0)
                {
                    // Đã thanh toán và có tiền hoàn
                    successMessage = $"Hủy đơn thành công. Số tiền hoàn lại: {policy.RefundAmount:N0} VNĐ (sẽ được xử lý trong 3-5 ngày làm việc).";
                }
                else
                {
                    // Đã thanh toán nhưng không được hoàn tiền (hủy quá gần)
                    successMessage = "Hủy đơn thành công. Do hủy quá gần giờ nhận xe nên không có tiền được hoàn lại.";
                }
                
                return ServiceResultDto<object>.SuccessResult(
                    new { RefundAmount = policy.RefundAmount, OriginalStatus = originalStatus },
                    successMessage);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"[ERROR] CancelRentalWithPolicyAsync: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                
                return ServiceResultDto<object>.FailureResult(
                    $"Có lỗi xảy ra khi hủy đơn: {ex.Message}");
            }
        }
    }
}

