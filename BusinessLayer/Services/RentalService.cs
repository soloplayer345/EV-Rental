using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;

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

        public async Task<ServiceResultDto<object>> CreateRentalAsync(int accountId, CreateRentalRequestDto request)
        {
            try
            {
                // Validate dates
                if (request.StartTime < DateTime.Now)
                {
                    return ServiceResultDto<object>.FailureResult("Thời gian bắt đầu phải lớn hơn thời gian hiện tại.");
                }

                if (request.ExpectedEndTime <= request.StartTime)
                {
                    return ServiceResultDto<object>.FailureResult("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
                }

                // Get vehicle
                var vehicle = await _vehicleService.GetVehicleByIdAsync(request.VehicleId);
                if (vehicle == null)
                {
                    return ServiceResultDto<object>.FailureResult("Không tìm thấy xe.");
                }

                if (vehicle.Status != VehicleStatus.Available)
                {
                    return ServiceResultDto<object>.FailureResult("Xe này hiện không khả dụng để thuê.");
                }

                // Calculate total cost
                TimeSpan duration = request.ExpectedEndTime - request.StartTime;
                decimal totalCost;

                if (duration.TotalDays >= 1)
                {
                    int days = (int)Math.Ceiling(duration.TotalDays);
                    totalCost = days * vehicle.PricePerDay;
                }
                else
                {
                    int hours = (int)Math.Ceiling(duration.TotalHours);
                    totalCost = hours * vehicle.PricePerHour;
                }

                // Create rental record
                var rentalRecord = new RentalRecord
                {
                    RenterId = accountId,
                    VehicleId = request.VehicleId,
                    PickupStationId = request.PickupStationId,
                    ReturnStationId = request.ReturnStationId,
                    StartTime = request.StartTime,
                    ExpectedEndTime = request.ExpectedEndTime,
                    BasePrice = totalCost,
                    TotalPrice = totalCost,
                    Status = RentalRecordStatus.Pending,
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

                return ServiceResultDto<object>.SuccessResult(null, "Đặt xe thành công! Vui lòng đến trạm nhận xe đúng giờ.");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<object>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        private string GenerateOtpCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
