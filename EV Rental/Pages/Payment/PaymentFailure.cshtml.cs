using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Payment
{
    public class PaymentFailureModel : PageModel
    {
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
        public string ResponseCode { get; set; } = string.Empty;

        public IActionResult OnGet(
            string message,
            string transactionId,
            string orderId,
            string paymentMethod,
            string orderDescription,
            decimal amount,
            string paymentTime,
            string responseCode)
        {
            Message = message ?? "Giao dịch thất bại";
            TransactionId = transactionId ?? "N/A";
            OrderId = orderId ?? "N/A";
            PaymentMethod = paymentMethod ?? "VNPay";
            OrderDescription = orderDescription ?? "Thanh toán thuê xe";
            Amount = amount;
            ResponseCode = responseCode ?? "N/A";
            
            if (!string.IsNullOrEmpty(paymentTime) && DateTime.TryParse(paymentTime, out var parsedTime))
            {
                PaymentTime = parsedTime;
            }
            else
            {
                PaymentTime = DateTime.Now;
            }

            return Page();
        }
    }
}

