using BusinessLayer.DTOs;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IRentalService
    {
        Task<ServiceResultDto<PendingBookingDto>> CalculateBookingCostAsync(int renterId, CreateRentalRequestDto request);
        Task<ServiceResultDto<RentalRecord>> CreatePendingRentalAsync(PendingBookingDto bookingDto, string otpCode);
        Task<ServiceResultDto<RentalRecord>> CreateRentalAfterPaymentAsync(PendingBookingDto bookingDto);
        Task<ServiceResultDto<RentalRecord>> ConfirmPendingRentalAsync(int rentalId);
        Task<RentalRecord?> GetPendingRentalByAccountAndVehicleAsync(int accountId, int vehicleId, DateTime startTime);
        Task<List<RentalRecord>> GetRentalsByRenterIdAsync(int renterId);
        Task<RentalRecord?> GetRentalByIdAsync(int rentalId);
        Task<List<StationDto>> GetAllStationsAsync();
        Task<ServiceResultDto<object>> CancelRentalAsync(int rentalId, int renterId);
        Task<RentalRecord?> GetRentalByOtpCodeAsync(string otpCode);
        Task<ServiceResultDto<RentalRecord>> VerifyOtpAndPickupVehicleAsync(string otpCode);
        Task<List<RentalRecord>> GetConfirmedRentalsByStationAsync(int stationId);
        Task<ServiceResultDto<CancellationPolicyDto>> GetCancellationPolicyAsync(int rentalId, int renterId);
        Task<ServiceResultDto<object>> CancelRentalWithPolicyAsync(int rentalId, int renterId, bool agreedToPolicy);
    }
}
