using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;

namespace BusinessLayer.Services
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResultDto<Payment>> CreatePaymentForRentalAsync(int rentalId, decimal amount, string method, string transactionRef)
        {
            try
            {
                var payment = new Payment
                {
                    RentalId = rentalId,
                    Amount = amount,
                    Method = method,
                    TransactionRef = transactionRef,
                    Status = "paid",
                    PaidAt = DateTime.Now,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    IsDeleted = false
                };

                var paymentRepo = _unitOfWork.GetRepository<Payment>();
                await paymentRepo.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResultDto<Payment>.SuccessResult(payment, "Tạo thanh toán thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<Payment>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResultDto<Payment>> GetPaymentByTransactionRefAsync(string transactionRef)
        {
            try
            {
                var paymentRepo = _unitOfWork.GetRepository<Payment>();
                var payments = await paymentRepo.GetAllAsync();
                var payment = payments.FirstOrDefault(p => p.TransactionRef == transactionRef);

                if (payment == null)
                {
                    return ServiceResultDto<Payment>.FailureResult("Không tìm thấy thanh toán.");
                }

                return ServiceResultDto<Payment>.SuccessResult(payment, "Lấy thông tin thanh toán thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<Payment>.FailureResult($"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
}
