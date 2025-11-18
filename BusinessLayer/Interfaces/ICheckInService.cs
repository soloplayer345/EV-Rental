using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface ICheckInService
    {
        Task<CheckInResultDto> ProcessCheckInAsync(CheckInDto checkInDto);
        Task<RentalRecordDto> GetBillingAsync(int rentalRecordId);
        Task<List<RentalRecordDto>> GetActiveRentalsAsync();
        Task<bool> ConfirmPaymentAsync(int rentalRecordId, decimal amount);
    }
}
