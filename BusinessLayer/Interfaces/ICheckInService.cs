using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ICheckInService
    {
        Task<bool> ProcessCheckInAsync(CheckInDto checkInDto);
        Task<RentalRecordDto> GetBillingAsync(int rentalRecordId);
        Task<bool> ConfirmPaymentAsync(int rentalRecordId, decimal amount);
    }
}
