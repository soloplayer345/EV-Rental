using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;

namespace BusinessLayer.Services
{
    public class PaymentService : IPaymentService
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

        /// <summary>
        /// Tạo payment record cho refund (hoàn tiền khi hủy đơn)
        /// </summary>
        public async Task<ServiceResultDto<Payment>> CreateRefundRecordAsync(int rentalId, decimal refundAmount)
        {
            try
            {
                // Nếu không có tiền hoàn, không tạo record
                if (refundAmount <= 0)
                {
                    return ServiceResultDto<Payment>.SuccessResult(
                        null, "Không có tiền hoàn lại.");
                }
                
                var paymentRepo = _unitOfWork.GetRepository<Payment>();
                
                // Tạo payment record cho refund
                var refundPayment = new Payment
                {
                    RentalId = rentalId,
                    Amount = -refundAmount,  // Số âm để đánh dấu là refund
                    Method = "refund",
                    TransactionRef = $"REFUND-{rentalId}-{DateTime.Now.Ticks}",
                    Status = "pending_refund",  // Chờ xử lý refund
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    IsDeleted = false
                };
                
                await paymentRepo.AddAsync(refundPayment);
                await _unitOfWork.SaveChangesAsync();
                
                return ServiceResultDto<Payment>.SuccessResult(
                    refundPayment, "Tạo record hoàn tiền thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<Payment>.FailureResult(
                    $"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
}
