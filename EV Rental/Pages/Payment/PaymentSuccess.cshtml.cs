using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessLayer.Entities;

namespace EV_Rental.Pages.Payment
{
    public class PaymentSuccessModel : PageModel
    {
        public string TransactionId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public RentalRecord? RentalRecord { get; set; }

        public IActionResult OnGet(
            string transactionId,
            string orderId,
            string paymentMethod,
            string orderDescription,
            decimal amount,
            string paymentTime,
            int? rentalId,
            int? vehicleId,
            string? otpCode,
            string? startTime,
            string? endTime)
        {
            TransactionId = transactionId ?? "N/A";
            OrderId = orderId ?? "N/A";
            PaymentMethod = paymentMethod ?? "VNPay";
            OrderDescription = orderDescription ?? "Thanh toán thuê xe";
            Amount = amount;
            
            if (!string.IsNullOrEmpty(paymentTime) && DateTime.TryParse(paymentTime, out var parsedTime))
            {
                PaymentTime = parsedTime;
            }
            else
            {
                PaymentTime = DateTime.Now;
            }

            // Load rental record if available
            if (rentalId.HasValue)
            {
                RentalRecord = new RentalRecord
                {
                    Id = rentalId.Value,
                    OtpCode = otpCode ?? "N/A",
                    StartTime = !string.IsNullOrEmpty(startTime) && DateTime.TryParse(startTime, out var st) ? st : DateTime.Now,
                    ExpectedEndTime = !string.IsNullOrEmpty(endTime) && DateTime.TryParse(endTime, out var et) ? et : DateTime.Now
                };
            }

            return Page();
        }
    }
}
