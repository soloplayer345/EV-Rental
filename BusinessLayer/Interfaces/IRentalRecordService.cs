using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IRentalRecordService
    {
        Task<IEnumerable<RentalRecordDto>> GetAllRentalRecordsAsync();
        Task<RentalRecordDto> GetRentalRecordByIdAsync(int id);
        Task UpdateRentalRecordAsync(RentalRecord rentalRecord);
        Task<IEnumerable<RentalRecordDto>> SearchRentalRecordsAsync(
            string searchQuery, 
            RentalRecordStatus? status, 
            DateTime? startDate, 
            DateTime? endDate);
        decimal CalculateTotalPrice(RentalRecord record);
    }
}
