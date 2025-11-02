using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using BusinessLayer.Services;
using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Payment
{
    public class VNPayReturnModel : PageModel
    {
        private readonly VNPaySettings _vnPaySettings;
        private readonly PaymentService _paymentService;
        private readonly RentalService _rentalService;

        public VNPayReturnModel(
            IOptions<VNPaySettings> vnPaySettings, 
            PaymentService paymentService,
            RentalService rentalService)
        {
            _vnPaySettings = vnPaySettings.Value;
            _paymentService = paymentService;
            _rentalService = rentalService;
        }

        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var vnpay = new VNPayLibrary();

            foreach (var (key, value) in Request.Query)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            // Get parameters from VNPay
            var vnp_OrderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = Request.Query["vnp_SecureHash"].ToString();
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_Amount = Convert.ToDecimal(vnpay.GetResponseData("vnp_Amount")) / 100;
            var vnp_BankCode = vnpay.GetResponseData("vnp_BankCode");

            var paymentTime = DateTime.Now;
            var bankName = GetBankName(vnp_BankCode);

            // Validate signature
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _vnPaySettings.HashSecret);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00")
                {
                    // Payment successful - Get booking data from session
                    var bookingDataJson = HttpContext.Session.GetString($"BookingData_{vnp_OrderId}");
                    
                    if (!string.IsNullOrEmpty(bookingDataJson))
                    {
                        var bookingDto = System.Text.Json.JsonSerializer.Deserialize<PendingBookingDto>(bookingDataJson);
                        
                        if (bookingDto != null)
                        {
                            // Create rental record now that payment is successful
                            var rentalResult = await _rentalService.CreateRentalAfterPaymentAsync(bookingDto);
                            
                            if (rentalResult.Success && rentalResult.Data != null)
                            {
                                var rental = rentalResult.Data;
                                
                                // Create payment record with rental ID
                                await _paymentService.CreatePaymentForRentalAsync(
                                    rental.Id, 
                                    vnp_Amount, 
                                    "vnpay", 
                                    vnp_OrderId
                                );
                                
                                // Clear session data
                                HttpContext.Session.Remove($"BookingData_{vnp_OrderId}");
                                
                                // Redirect to success page with rental details
                                return RedirectToPage("/Payment/PaymentSuccess", new
                                {
                                    transactionId = vnp_TransactionId,
                                    orderId = vnp_OrderId,
                                    paymentMethod = bankName,
                                    orderDescription = vnp_OrderInfo,
                                    amount = vnp_Amount,
                                    paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                                    rentalId = rental.Id,
                                    vehicleId = rental.VehicleId,
                                    otpCode = rental.OtpCode,
                                    startTime = rental.StartTime?.ToString("yyyy-MM-ddTHH:mm:ss"),
                                    endTime = rental.ExpectedEndTime?.ToString("yyyy-MM-ddTHH:mm:ss")
                                });
                            }
                            else
                            {
                                // Failed to create rental
                                return RedirectToPage("/Payment/PaymentFailure", new
                                {
                                    message = "Thanh toán thành công nhưng không thể tạo đơn thuê: " + rentalResult.Message,
                                    transactionId = vnp_TransactionId,
                                    orderId = vnp_OrderId,
                                    paymentMethod = bankName,
                                    orderDescription = vnp_OrderInfo,
                                    amount = vnp_Amount,
                                    paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                                    responseCode = "RENTAL_CREATE_FAILED"
                                });
                            }
                        }
                    }
                    
                    // No booking data found
                    return RedirectToPage("/Payment/PaymentFailure", new
                    {
                        message = "Không tìm thấy thông tin đặt xe",
                        transactionId = vnp_TransactionId,
                        orderId = vnp_OrderId,
                        paymentMethod = bankName,
                        orderDescription = vnp_OrderInfo,
                        amount = vnp_Amount,
                        paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        responseCode = "BOOKING_DATA_NOT_FOUND"
                    });
                }
                else
                {
                    // Payment failed - just clear session and redirect to failure page
                    var message = GetVNPayResponseMessage(vnp_ResponseCode);
                    
                    // Clear session data if exists
                    HttpContext.Session.Remove($"BookingData_{vnp_OrderId}");

                    // Redirect to failure page (no rental or payment created)
                    return RedirectToPage("/Payment/PaymentFailure", new
                    {
                        message = message,
                        transactionId = vnp_TransactionId,
                        orderId = vnp_OrderId,
                        paymentMethod = bankName,
                        orderDescription = vnp_OrderInfo,
                        amount = vnp_Amount,
                        paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        responseCode = vnp_ResponseCode
                    });
                }
            }
            else
            {
                // Invalid signature - clear session and redirect to failure page
                var message = "Chữ ký không hợp lệ. Giao dịch có thể bị giả mạo.";
                
                // Clear session data if exists
                HttpContext.Session.Remove($"BookingData_{vnp_OrderId}");
                
                // Redirect to failure page
                return RedirectToPage("/Payment/PaymentFailure", new
                {
                    message = message,
                    transactionId = vnp_TransactionId,
                    orderId = vnp_OrderId,
                    paymentMethod = bankName,
                    orderDescription = vnp_OrderInfo,
                    amount = vnp_Amount,
                    paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    responseCode = "INVALID_SIGNATURE"
                });
            }
        }

        private string GetVNPayResponseMessage(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
                "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá giới hạn giao dịch trong ngày.",
                "75" => "Ngân hàng thanh toán đang bảo trì.",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch",
                _ => "Giao dịch thất bại. Mã lỗi: " + responseCode
            };
        }

        private string GetBankName(string bankCode)
        {
            return bankCode switch
            {
                "NCB" => "Ngân hàng NCB",
                "VISA" => "Thẻ quốc tế VISA",
                "MASTERCARD" => "Thẻ quốc tế MasterCard",
                "JCB" => "Thẻ quốc tế JCB",
                "UPI" => "Thẻ quốc tế UPI",
                "VIB" => "Ngân hàng VIB",
                "VIETCOMBANK" => "Ngân hàng Vietcombank",
                "BIDV" => "Ngân hàng BIDV",
                "TECHCOMBANK" => "Ngân hàng Techcombank",
                "VIETINBANK" => "Ngân hàng VietinBank",
                "MB" => "Ngân hàng MB",
                "ACB" => "Ngân hàng ACB",
                "SHB" => "Ngân hàng SHB",
                "VNMART" => "Ví VnMart",
                _ => bankCode
            };
        }
    }
}
