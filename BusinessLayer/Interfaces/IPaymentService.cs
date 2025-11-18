using BusinessLayer.DTOs;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IPaymentService
    {
        Task<ServiceResultDto<Payment>> CreatePaymentForRentalAsync(int rentalId, decimal amount, string method, string transactionRef);
        Task<ServiceResultDto<Payment>> GetPaymentByTransactionRefAsync(string transactionRef);
        Task<ServiceResultDto<Payment>> CreateRefundRecordAsync(int rentalId, decimal refundAmount);
    }
}
