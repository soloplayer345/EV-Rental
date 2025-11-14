using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using BusinessLayer.Services;
using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Payment
{
    [AllowAnonymous]
    public class VNPayReturnModel : PageModel
    {
        private readonly VNPaySettings _vnPaySettings;
        private readonly PaymentService _paymentService;
        private readonly RentalService _rentalService;
        private readonly VehicleService _vehicleService;
        private readonly AccountService _accountService;
        private readonly IEmailSender _emailSender;

        public VNPayReturnModel(
            IOptions<VNPaySettings> vnPaySettings, 
            PaymentService paymentService,
            RentalService rentalService,
            VehicleService vehicleService,
            AccountService accountService,
            IEmailSender emailSender)
        {
            _vnPaySettings = vnPaySettings.Value;
            _paymentService = paymentService;
            _rentalService = rentalService;
            _vehicleService = vehicleService;
            _accountService = accountService;
            _emailSender = emailSender;
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
                    // Payment successful - Get pending rental ID from session
                    var rentalIdStr = HttpContext.Session.GetString($"PendingRentalId_{vnp_OrderId}");
                    
                    if (!string.IsNullOrEmpty(rentalIdStr) && int.TryParse(rentalIdStr, out int rentalId))
                    {
                        // Confirm the pending rental
                        var confirmResult = await _rentalService.ConfirmPendingRentalAsync(rentalId);
                        
                        if (confirmResult.Success && confirmResult.Data != null)
                        {
                            var rental = confirmResult.Data;
                            
                            // Create payment record with rental ID
                            await _paymentService.CreatePaymentForRentalAsync(
                                rental.Id, 
                                vnp_Amount, 
                                "vnpay", 
                                vnp_OrderId
                            );
                            
                            // Send OTP email to renter
                            try
                            {
                                var renterAccount = await _accountService.GetByIdAsync(rental.RenterId);
                                if (renterAccount != null)
                                {
                                    await SendOtpEmailAsync(renterAccount.Email, renterAccount.FullName, rental);
                                    Console.WriteLine($"[EMAIL] OTP email sent to {renterAccount.Email}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[EMAIL ERROR] Failed to send OTP email: {ex.Message}");
                                // Don't fail the payment flow if email fails
                            }
                            
                            // Clear session data
                            HttpContext.Session.Remove($"PendingRentalId_{vnp_OrderId}");
                            
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
                            // Failed to confirm rental
                            return RedirectToPage("/Payment/PaymentFailure", new
                            {
                                message = "Thanh toán thành công nhưng không thể xác nhận đơn thuê: " + confirmResult.Message,
                                transactionId = vnp_TransactionId,
                                orderId = vnp_OrderId,
                                paymentMethod = bankName,
                                orderDescription = vnp_OrderInfo,
                                amount = vnp_Amount,
                                paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                                responseCode = "RENTAL_CONFIRM_FAILED"
                            });
                        }
                    }
                    
                    // No pending rental found
                    return RedirectToPage("/Payment/PaymentFailure", new
                    {
                        message = "Không tìm thấy thông tin đơn thuê chờ thanh toán",
                        transactionId = vnp_TransactionId,
                        orderId = vnp_OrderId,
                        paymentMethod = bankName,
                        orderDescription = vnp_OrderInfo,
                        amount = vnp_Amount,
                        paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        responseCode = "PENDING_RENTAL_NOT_FOUND"
                    });
                }
                else
                {
                    // Payment failed - cancel pending rental and redirect to failure page
                    var message = GetVNPayResponseMessage(vnp_ResponseCode);
                    
                    // Cancel the pending rental if exists
                    var rentalIdStr = HttpContext.Session.GetString($"PendingRentalId_{vnp_OrderId}");
                    if (!string.IsNullOrEmpty(rentalIdStr) && int.TryParse(rentalIdStr, out int rentalId))
                    {
                        try
                        {
                            var rental = await _rentalService.GetRentalByIdAsync(rentalId);
                            if (rental != null && rental.Status == DataAccessLayer.Enums.RentalRecordStatus.Pending)
                            {
                                await _rentalService.CancelRentalAsync(rentalId, rental.RenterId);
                            }
                        }
                        catch (Exception)
                        {
                            // Log error but continue
                        }
                    }
                    
                    // Clear session data
                    HttpContext.Session.Remove($"PendingRentalId_{vnp_OrderId}");

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
                        responseCode = vnp_ResponseCode
                    });
                }
            }
            else
            {
                // Invalid signature - cancel pending rental and redirect to failure page
                var message = "Chữ ký không hợp lệ. Giao dịch có thể bị giả mạo.";
                
                // Cancel the pending rental if exists
                var rentalIdStr = HttpContext.Session.GetString($"PendingRentalId_{vnp_OrderId}");
                if (!string.IsNullOrEmpty(rentalIdStr) && int.TryParse(rentalIdStr, out int rentalId))
                {
                    try
                    {
                        var rental = await _rentalService.GetRentalByIdAsync(rentalId);
                        if (rental != null && rental.Status == DataAccessLayer.Enums.RentalRecordStatus.Pending)
                        {
                            await _rentalService.CancelRentalAsync(rentalId, rental.RenterId);
                        }
                    }
                    catch (Exception)
                    {
                        // Log error but continue
                    }
                }
                
                // Clear session data
                HttpContext.Session.Remove($"PendingRentalId_{vnp_OrderId}");
                
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

        private async Task SendOtpEmailAsync(string toEmail, string customerName, DataAccessLayer.Entities.RentalRecord rentalRecord)
        {
            var subject = $"🔋 Mã OTP #{rentalRecord.Id} - Thanh Toán Thành Công - EV Rental";
            
            // Get vehicle info
            var vehicle = await _vehicleService.GetVehicleByIdAsync(rentalRecord.VehicleId);
            var vehicleName = vehicle?.Name ?? "N/A";
            
            // Get station info
            var stations = await _rentalService.GetAllStationsAsync();
            var pickupStation = stations.FirstOrDefault(s => s.Id == rentalRecord.PickupStationId)?.Name ?? "N/A";
            var returnStation = stations.FirstOrDefault(s => s.Id == rentalRecord.ReturnStationId)?.Name ?? "N/A";
            
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 40px 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; }}
        .content {{ padding: 30px; }}
        .greeting {{ font-size: 18px; color: #1f2937; margin-bottom: 15px; }}
        .success-badge {{ display: inline-block; background: #d1fae5; color: #065f46; padding: 8px 16px; border-radius: 20px; font-weight: 600; margin: 15px 0; }}
        .otp-section {{ background: linear-gradient(135deg, #eff6ff 0%, #dbeafe 100%); border: 3px dashed #2563eb; padding: 25px; text-align: center; margin: 25px 0; border-radius: 12px; }}
        .otp-label {{ font-size: 14px; color: #6b7280; margin: 0 0 10px 0; font-weight: 500; }}
        .otp-code {{ font-size: 42px; font-weight: bold; color: #2563eb; letter-spacing: 10px; margin: 15px 0; font-family: 'Courier New', monospace; }}
        .info-box {{ background: #f9fafb; padding: 20px; margin: 20px 0; border-radius: 10px; border-left: 4px solid #10b981; }}
        .info-box h3 {{ margin: 0 0 15px 0; color: #1f2937; font-size: 16px; }}
        .info-row {{ display: flex; padding: 8px 0; border-bottom: 1px solid #e5e7eb; }}
        .info-row:last-child {{ border-bottom: none; }}
        .info-label {{ font-weight: 600; color: #6b7280; width: 140px; flex-shrink: 0; }}
        .info-value {{ color: #1f2937; flex-grow: 1; }}
        .cost-box {{ background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 20px; border-radius: 10px; text-align: center; margin: 20px 0; }}
        .cost-label {{ font-size: 14px; opacity: 0.9; margin: 0; }}
        .cost-amount {{ font-size: 32px; font-weight: bold; margin: 10px 0; }}
        .warning-box {{ background: #fef3c7; padding: 18px; border-radius: 10px; border-left: 4px solid #f59e0b; margin: 20px 0; }}
        .warning-box strong {{ color: #92400e; }}
        .warning-box ul {{ margin: 10px 0 0 0; padding-left: 20px; color: #78350f; }}
        .warning-box li {{ margin: 6px 0; }}
        .btn {{ display: inline-block; padding: 14px 28px; background: #2563eb; color: white; text-decoration: none; border-radius: 8px; margin: 20px 0; font-weight: 600; }}
        .btn:hover {{ background: #1d4ed8; }}
        .footer {{ background: #1f2937; color: #9ca3af; text-align: center; padding: 25px; font-size: 13px; }}
        .footer p {{ margin: 5px 0; }}
        .divider {{ height: 1px; background: linear-gradient(to right, transparent, #e5e7eb, transparent); margin: 20px 0; }}
        .status-badge {{ display: inline-block; padding: 6px 12px; background: #d1fae5; color: #065f46; border-radius: 6px; font-size: 13px; font-weight: 600; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ THANH TOÁN THÀNH CÔNG</h1>
            <p>Đơn Thuê Xe Đã Được Xác Nhận</p>
        </div>
        
        <div class='content'>
            <p class='greeting'>Xin chào <strong>{customerName}</strong>,</p>
            <p>Chúc mừng! Thanh toán của bạn đã được xử lý thành công. Đơn thuê xe của bạn đã được xác nhận! 🎉</p>
            
            <div class='success-badge'>✓ ĐÃ THANH TOÁN THÀNH CÔNG</div>
            
            <div class='otp-section'>
                <p class='otp-label'>🔐 MÃ OTP XÁC NHẬN NHẬN XE</p>
                <div class='otp-code'>{rentalRecord.OtpCode}</div>
                <p style='margin: 10px 0 0 0; font-size: 13px; color: #6b7280;'>Vui lòng xuất trình mã này khi nhận xe</p>
            </div>

            <div class='info-box'>
                <h3>📋 Thông Tin Chi Tiết Đơn Thuê</h3>
                <div class='info-row'>
                    <span class='info-label'>Mã đơn hàng:</span>
                    <span class='info-value'><strong>#{rentalRecord.Id}</strong></span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Xe thuê:</span>
                    <span class='info-value'>{vehicleName}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Trạm nhận xe:</span>
                    <span class='info-value'>{pickupStation}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Trạm trả xe:</span>
                    <span class='info-value'>{returnStation}</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Thời gian nhận:</span>
                    <span class='info-value'><strong>{rentalRecord.StartTime:dd/MM/yyyy HH:mm}</strong></span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Thời gian trả:</span>
                    <span class='info-value'><strong>{rentalRecord.ExpectedEndTime:dd/MM/yyyy HH:mm}</strong></span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>Trạng thái:</span>
                    <span class='info-value'><span class='status-badge'>✓ Đã xác nhận</span></span>
                </div>
            </div>

            <div class='cost-box'>
                <p class='cost-label'>💰 Đã Thanh Toán</p>
                <div class='cost-amount'>{rentalRecord.TotalPrice:N0} VNĐ</div>
                <p style='margin: 0; font-size: 13px; opacity: 0.9;'>Thanh toán qua VNPay</p>
            </div>

            <div class='warning-box'>
                <p style='margin: 0 0 8px 0;'><strong>📌 LƯU Ý QUAN TRỌNG</strong></p>
                <ul>
                    <li><strong>Mang theo mã OTP</strong> khi đến nhận xe tại trạm</li>
                    <li>Xuất trình mã OTP cho nhân viên để xác nhận danh tính</li>
                    <li>Đến đúng giờ nhận xe: <strong>{rentalRecord.StartTime:dd/MM/yyyy HH:mm}</strong></li>
                    <li>Mang theo <strong>CMND/CCCD và Giấy phép lái xe</strong> (bản gốc)</li>
                    <li>Kiểm tra kỹ xe trước khi nhận</li>
                </ul>
            </div>

            <div class='divider'></div>

            <p style='text-align: center; margin: 25px 0;'>
                <a href='https://localhost:7158/Renter/MyTrips' class='btn'>👉 Xem Chi Tiết Đơn Thuê</a>
            </p>

            <p style='color: #6b7280; font-size: 13px; text-align: center; margin: 20px 0 0 0;'>
                Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ hotline: <strong style='color: #2563eb;'>1900-xxxx</strong>
            </p>
        </div>
        
        <div class='footer'>
            <p style='font-weight: 600; color: white; margin-bottom: 10px;'>🔋 EV RENTAL SYSTEM</p>
            <p>Email này được gửi tự động từ hệ thống</p>
            <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
            <p style='margin-top: 15px;'>&copy; 2025 EV Rental System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            await _emailSender.SendAsync(toEmail, subject, htmlBody);
        }
    }
}
