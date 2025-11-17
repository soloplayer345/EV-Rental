using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using EV_Rental.Helpers;
using DataAccessLayer.Enums;

namespace EV_Rental.Pages.Payment
{
    [AllowAnonymous]
    public class MoMoReturnModel : PageModel
    {
        private readonly MoMoSettings _momoSettings;
        private readonly IPaymentService _paymentService;
        private readonly IRentalService _rentalService;
        private readonly IVehicleService _vehicleService;
        private readonly IAccountService _accountService;
        private readonly IEmailSender _emailSender;

        public MoMoReturnModel(
            IOptions<MoMoSettings> momoSettings,
            IPaymentService paymentService,
            IRentalService rentalService,
            IVehicleService vehicleService,
            IAccountService accountService,
            IEmailSender emailSender)
        {
            _momoSettings = momoSettings.Value;
            _paymentService = paymentService;
            _rentalService = rentalService;
            _vehicleService = vehicleService;
            _accountService = accountService;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Console.WriteLine("=== MOMO CALLBACK RECEIVED ===");
            
            var partnerCode = Request.Query["partnerCode"].ToString();
            var orderId = Request.Query["orderId"].ToString();
            var requestId = Request.Query["requestId"].ToString();
            var amount = Request.Query["amount"].ToString();
            var orderInfo = Request.Query["orderInfo"].ToString();
            var orderType = Request.Query["orderType"].ToString();
            var transId = Request.Query["transId"].ToString();
            var resultCode = Request.Query["resultCode"].ToString();
            var message = Request.Query["message"].ToString();
            var payType = Request.Query["payType"].ToString();
            var responseTime = Request.Query["responseTime"].ToString();
            var extraData = Request.Query["extraData"].ToString();
            var signature = Request.Query["signature"].ToString();

            Console.WriteLine($"OrderId: {orderId}");
            Console.WriteLine($"ResultCode: {resultCode}");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine($"Amount: {amount}");
            Console.WriteLine($"TransId: {transId}");

            var paymentTime = DateTime.Now;

            decimal amountDecimal = 0;
            if (!string.IsNullOrEmpty(amount))
            {
                amountDecimal = decimal.Parse(amount);
            }

            if (resultCode == "0")
            {
                Console.WriteLine("[SUCCESS] Payment successful, confirming rental...");
                var rentalIdStr = HttpContext.Session.GetString($"PendingRentalId_{orderId}");
                Console.WriteLine($"[SESSION] PendingRentalId found: {!string.IsNullOrEmpty(rentalIdStr)}");
                
                if (!string.IsNullOrEmpty(rentalIdStr) && int.TryParse(rentalIdStr, out int rentalId))
                {
                    Console.WriteLine($"[RENTAL] Confirming rental #{rentalId}...");
                    var confirmResult = await _rentalService.ConfirmPendingRentalAsync(rentalId);
                    Console.WriteLine($"[RENTAL] Result: {confirmResult.Success}, Message: {confirmResult.Message}");
                    
                    if (confirmResult.Success && confirmResult.Data != null)
                    {
                        var rental = confirmResult.Data;
                        
                        // Create payment record
                        await _paymentService.CreatePaymentForRentalAsync(rental.Id, amountDecimal, "momo", orderId);
                        Console.WriteLine($"[PAYMENT] Payment record created for rental #{rental.Id}");
                        
                        // Send OTP email
                        try
                        {
                            var renterAccount = await _accountService.GetAccountByIdAsync(rental.RenterId);
                            if (renterAccount != null && !string.IsNullOrEmpty(renterAccount.Email))
                            {
                                await SendOtpEmailAsync(renterAccount.Email, renterAccount.FullName, rental);
                                Console.WriteLine($"[EMAIL] OTP email sent to {renterAccount.Email}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[EMAIL ERROR] Failed to send OTP email: {ex.Message}");
                            // Continue even if email fails
                        }
                        
                        // Clear session
                        HttpContext.Session.Remove($"PendingRentalId_{orderId}");
                        Console.WriteLine("[SUCCESS] Redirecting to success page");
                        
                        return RedirectToPage("/Payment/PaymentSuccess", new
                        {
                            transactionId = transId,
                            orderId = orderId,
                            paymentMethod = "MoMo",
                            orderDescription = orderInfo,
                            amount = amountDecimal,
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
                        return RedirectToPage("/Payment/PaymentFailure", new
                        {
                            message = "Thanh toán thành công nhưng không thể xác nhận đơn thuê: " + confirmResult.Message,
                            transactionId = transId,
                            orderId = orderId,
                            paymentMethod = "MoMo",
                            orderDescription = orderInfo,
                            amount = amountDecimal,
                            paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                            responseCode = "RENTAL_CONFIRM_FAILED"
                        });
                    }
                }
                
                return RedirectToPage("/Payment/PaymentFailure", new
                {
                    message = "Không tìm thấy thông tin đơn thuê chờ thanh toán",
                    transactionId = transId,
                    orderId = orderId,
                    paymentMethod = "MoMo",
                    orderDescription = orderInfo,
                    amount = amountDecimal,
                    paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    responseCode = "PENDING_RENTAL_NOT_FOUND"
                });
            }
            else
            {
                Console.WriteLine($"[FAILURE] Payment failed with code: {resultCode}");
                var errorMessage = GetMoMoResponseMessage(resultCode);
                
                // Cancel the pending rental if exists
                var rentalIdStr = HttpContext.Session.GetString($"PendingRentalId_{orderId}");
                if (!string.IsNullOrEmpty(rentalIdStr) && int.TryParse(rentalIdStr, out int rentalId))
                {
                    try
                    {
                        var rental = await _rentalService.GetRentalByIdAsync(rentalId);
                        if (rental != null && rental.Status == DataAccessLayer.Enums.RentalRecordStatus.Pending)
                        {
                            await _rentalService.CancelRentalAsync(rentalId, rental.RenterId);
                            Console.WriteLine($"[CANCELLED] Pending rental #{rentalId} cancelled due to payment failure");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to cancel pending rental: {ex.Message}");
                    }
                }
                
                HttpContext.Session.Remove($"PendingRentalId_{orderId}");

                return RedirectToPage("/Payment/PaymentFailure", new
                {
                    message = errorMessage,
                    transactionId = transId,
                    orderId = orderId,
                    paymentMethod = "MoMo",
                    orderDescription = orderInfo,
                    amount = amountDecimal,
                    paymentTime = paymentTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    responseCode = resultCode
                });
            }
        }

        private string GetMoMoResponseMessage(string resultCode)
        {
            return resultCode switch
            {
                "0" => "Giao dịch thành công",
                "9000" => "Giao dịch được khởi tạo, chờ xác nhận",
                "7000" => "Giao dịch bị từ chối",
                "7001" => "Tài khoản không đủ tiền",
                "1000" => "Lỗi hệ thống",
                "4100" => "Giao dịch bị từ chối",
                _ => $"Giao dịch thất bại. Mã: {resultCode}"
            };
        }

        private async Task SendOtpEmailAsync(string toEmail, string customerName, DataAccessLayer.Entities.RentalRecord rentalRecord)
        {
            var subject = $"🎉 Mã OTP #{rentalRecord.Id} - Thanh toán thành công - EV Rental";
            
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
        .Status-badge {{ display: inline-block; padding: 6px 12px; background: #d1fae5; color: #065f46; border-radius: 6px; font-size: 13px; font-weight: 600; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ THANH TOÁN THÀNH CÔNG</h1>
            <p>Đơn thuê xe đã được xác nhận</p>
        </div>
        
        <div class='content'>
            <p class='greeting'>Xin chào <strong>{customerName}</strong>,</p>
            <p>Chúc mừng! Thanh toán của bạn đã được xử lý thành công. Đơn thuê xe của bạn đã được xác nhận! 🎉</p>

            <div class='success-badge'>🎯 ĐÃ THANH TOÁN THÀNH CÔNG</div>
            
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
                    <span class='info-value'><span class='status-badge'>✅ ĐÃ XÁC NHẬN</span></span>
                </div>
            </div>

            <div class='cost-box'>
                <p class='cost-label'>💰 ĐÃ THANH TOÁN</p>
                <div class='cost-amount'>{rentalRecord.TotalPrice:N0} VNĐ</div>
                <p style='margin: 0; font-size: 13px; opacity: 0.9;'>Thanh toán qua MoMo</p>
            </div>

            <div class='warning-box'>
                <p style='margin: 0 0 8px 0;'><strong>⚠️ LƯU Ý QUAN TRỌNG</strong></p>
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
                <a href='https://localhost:7158/Renter/MyTrips' class='btn'>📄 Xem Chi Tiết Đơn Thuê</a>
            </p>

            <p style='color: #6b7280; font-size: 13px; text-align: center; margin: 20px 0 0 0;'>
                Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ hotline: <strong style='color: #2563eb;'>1900-xxxx</strong>
            </p>
        </div>
        
        <div class='footer'>
            <p style='font-weight: 600; color: white; margin-bottom: 10px;'>🚗 EV RENTAL SYSTEM</p>
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

