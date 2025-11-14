using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using BusinessLayer.Services;
using BusinessLayer.DTOs;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Renter
{
    public class ConfirmRentalModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private readonly RentalService _rentalService;
        private readonly PaymentService _paymentService;
        private readonly AccountService _accountService;
        private readonly VNPaySettings _vnPaySettings;
        private readonly MoMoSettings _momoSettings;
        private readonly IEmailSender _emailSender;

        public ConfirmRentalModel(
            VehicleService vehicleService, 
            RentalService rentalService,
            PaymentService paymentService,
            AccountService accountService,
            IOptions<VNPaySettings> vnPaySettings,
            IOptions<MoMoSettings> momoSettings,
            IEmailSender emailSender)
        {
            _vehicleService = vehicleService;
            _rentalService = rentalService;
            _paymentService = paymentService;
            _accountService = accountService;
            _vnPaySettings = vnPaySettings.Value;
            _momoSettings = momoSettings.Value;
            _emailSender = emailSender;
        }

        public VehicleDto? Vehicle { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PickupStationId { get; set; }
        public int ReturnStationId { get; set; }
        public string? Notes { get; set; }
        
        public string PickupStationName { get; set; } = string.Empty;
        public string ReturnStationName { get; set; } = string.Empty;
        public string RentalDuration { get; set; } = string.Empty;
        
        // Cost breakdown
        public decimal BasePrice { get; set; }
        public decimal ReservationFee { get; set; }
        public decimal DepositAmount { get; set; } = 3000000; // 3 triệu VNĐ
        public decimal TotalCost { get; set; }
        
        // Additional info
        public bool HasReservationFee { get; set; }
        public bool IsHourlyRental { get; set; }
        public int RentalDays { get; set; }
        public int RentalHours { get; set; }
        public int DaysUntilPickup { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        [BindProperty]
        public string PaymentMethod { get; set; } = "momo"; // Default: MoMo

        public async Task<IActionResult> OnGetAsync(
            int vehicleId,
            DateTime startDate,
            DateTime endDate,
            int pickupStationId,
            int returnStationId,
            string? notes)
        {
            // Check if user is logged in
            var accountId = SessionHelper.GetAccountId(HttpContext);
            if (accountId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Store data
            VehicleId = vehicleId;
            StartDate = startDate;
            EndDate = endDate;
            PickupStationId = pickupStationId;
            ReturnStationId = returnStationId;
            Notes = notes;

            // Load vehicle
            Vehicle = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            
            if (Vehicle == null)
            {
                return RedirectToPage("/Renter/Index");
            }

            // Check if vehicle is available
            if (Vehicle.Status != VehicleStatus.Available)
            {
                ErrorMessage = "Xe này hiện không khả dụng để thuê.";
                return Page();
            }

            // Load station names
            var stations = await _rentalService.GetAllStationsAsync();
            PickupStationName = stations.FirstOrDefault(s => s.Id == pickupStationId)?.Name ?? "N/A";
            ReturnStationName = stations.FirstOrDefault(s => s.Id == returnStationId)?.Name ?? "N/A";

            // Calculate rental duration
            TimeSpan duration = endDate - startDate;
            if (duration.TotalDays >= 1)
            {
                RentalDays = (int)Math.Ceiling(duration.TotalDays);
                RentalDuration = $"{RentalDays} ngày";
                IsHourlyRental = false;
            }
            else
            {
                RentalHours = (int)Math.Ceiling(duration.TotalHours);
                RentalDuration = $"{RentalHours} giờ";
                IsHourlyRental = true;
            }

            // Calculate base price
            if (IsHourlyRental)
            {
                BasePrice = RentalHours * Vehicle.PricePerHour;
            }
            else
            {
                BasePrice = RentalDays * Vehicle.PricePerDay;
            }

            // Calculate days until pickup
            TimeSpan untilPickup = startDate - DateTime.Now;
            DaysUntilPickup = (int)Math.Ceiling(untilPickup.TotalDays);

            // Calculate reservation fee (if booking more than 1 day in advance)
            HasReservationFee = DaysUntilPickup > 1;
            if (HasReservationFee)
            {
                // Reservation fee = 10% of base price, minimum 50,000 VND
                ReservationFee = Math.Max(BasePrice * 0.1m, 50000);
            }
            else
            {
                ReservationFee = 0;
            }

            // Calculate total cost
            TotalCost = BasePrice + ReservationFee + DepositAmount;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
            int vehicleId,
            DateTime startDate,
            DateTime endDate,
            int pickupStationId,
            int returnStationId,
            string? notes)
        {
            // Check if user is logged in
            var accountId = SessionHelper.GetAccountId(HttpContext);
            if (accountId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Reload data for display in case of error
            VehicleId = vehicleId;
            StartDate = startDate;
            EndDate = endDate;
            PickupStationId = pickupStationId;
            ReturnStationId = returnStationId;
            Notes = notes;

            Vehicle = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            
            if (Vehicle == null)
            {
                return RedirectToPage("/Renter/Index");
            }

            // Recalculate all values
            var stations = await _rentalService.GetAllStationsAsync();
            PickupStationName = stations.FirstOrDefault(s => s.Id == pickupStationId)?.Name ?? "N/A";
            ReturnStationName = stations.FirstOrDefault(s => s.Id == returnStationId)?.Name ?? "N/A";

            TimeSpan duration = endDate - startDate;
            if (duration.TotalDays >= 1)
            {
                RentalDays = (int)Math.Ceiling(duration.TotalDays);
                RentalDuration = $"{RentalDays} ngày";
                IsHourlyRental = false;
                BasePrice = RentalDays * Vehicle.PricePerDay;
            }
            else
            {
                RentalHours = (int)Math.Ceiling(duration.TotalHours);
                RentalDuration = $"{RentalHours} giờ";
                IsHourlyRental = true;
                BasePrice = RentalHours * Vehicle.PricePerHour;
            }

            TimeSpan untilPickup = startDate - DateTime.Now;
            DaysUntilPickup = (int)Math.Ceiling(untilPickup.TotalDays);
            HasReservationFee = DaysUntilPickup > 1;
            
            if (HasReservationFee)
            {
                ReservationFee = Math.Max(BasePrice * 0.1m, 50000);
            }
            else
            {
                ReservationFee = 0;
            }

            TotalCost = BasePrice + ReservationFee + DepositAmount;

            // Create rental request
            var request = new CreateRentalRequestDto
            {
                VehicleId = vehicleId,
                PickupStationId = pickupStationId,
                ReturnStationId = returnStationId,
                StartTime = startDate,
                ExpectedEndTime = endDate,
                Notes = notes
            };

            // Calculate booking cost (don't create rental yet)
            var result = await _rentalService.CalculateBookingCostAsync(accountId.Value, request);

            if (!result.Success || result.Data == null)
            {
                ErrorMessage = result.Message;
                return Page();
            }

            var bookingDto = result.Data;

            // Generate transaction reference and OTP code
            var transactionRef = $"BOOKING{accountId.Value}_{DateTime.Now:yyyyMMddHHmmss}";
            var otpCode = GenerateOtpCode();

            // Process based on payment method
            if (PaymentMethod == "momo")
            {
                // Create pending rental first (before payment)
                var pendingRentalResult = await _rentalService.CreatePendingRentalAsync(bookingDto, otpCode);
                
                if (!pendingRentalResult.Success || pendingRentalResult.Data == null)
                {
                    ErrorMessage = $"Không thể tạo đơn thuê: {pendingRentalResult.Message}";
                    return Page();
                }

                var pendingRental = pendingRentalResult.Data;

                // Store rental ID in session to look up after payment
                HttpContext.Session.SetString($"PendingRentalId_{transactionRef}", pendingRental.Id.ToString());

                // Redirect to MoMo
                try
                {
                    var momoUrl = await CreateMoMoPaymentUrl(transactionRef, TotalCost, $"Thanh toan thue xe {Vehicle?.Name}");
                    return Redirect(momoUrl);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Không thể tạo thanh toán MoMo: {ex.Message}";
                    return Page();
                }
            }
            else if (PaymentMethod == "vnpay")
            {
                // Create pending rental first (before payment)
                var pendingRentalResult = await _rentalService.CreatePendingRentalAsync(bookingDto, otpCode);
                
                if (!pendingRentalResult.Success || pendingRentalResult.Data == null)
                {
                    ErrorMessage = $"Không thể tạo đơn thuê: {pendingRentalResult.Message}";
                    return Page();
                }

                var pendingRental = pendingRentalResult.Data;

                // Store rental ID in session to look up after payment
                HttpContext.Session.SetString($"PendingRentalId_{transactionRef}", pendingRental.Id.ToString());

                // Redirect to VNPay
                var vnpayUrl = CreateVNPayPaymentUrl(transactionRef, TotalCost, $"Thanh toan thue xe {Vehicle?.Name}");
                return Redirect(vnpayUrl);
            }
            else if (PaymentMethod == "cash")
            {
                // For cash payment, create rental directly with pending payment status
                var rentalResult = await _rentalService.CreateRentalAfterPaymentAsync(bookingDto);
                
                if (!rentalResult.Success)
                {
                    ErrorMessage = rentalResult.Message;
                    return Page();
                }

                var rentalRecord = rentalResult.Data;

                // Get renter account to send email
                var renterAccount = await _accountService.GetAccountByIdAsync(accountId.Value);

                if (renterAccount != null && !string.IsNullOrEmpty(renterAccount.Email) && rentalRecord != null)
                {
                    try
                    {
                        // Send OTP email
                        await SendOtpEmailAsync(renterAccount.Email, renterAccount.FullName, rentalRecord);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't block the process
                        Console.WriteLine($"Failed to send OTP email: {ex.Message}");
                    }
                }

                // Redirect to success page with cash payment info
                TempData["SuccessMessage"] = $"Đặt xe thành công! Mã OTP đã được gửi đến email {renterAccount?.Email}. Vui lòng thanh toán bằng tiền mặt khi nhận xe tại trạm.";
                TempData["PaymentMethod"] = "cash";
                TempData["TotalAmount"] = TotalCost.ToString("N0");
                return RedirectToPage("/Renter/MyTrips");
            }
            else
            {
                ErrorMessage = "Phương thức thanh toán không hợp lệ.";
                return Page();
            }
        }

        private string CreateVNPayPaymentUrl(string orderId, decimal amount, string orderInfo)
        {
            var vnpay = new VNPayLibrary();
            
            // Lấy IP address
            string ipAddr = GetIpAddress();
            if (string.IsNullOrEmpty(ipAddr))
            {
                ipAddr = "127.0.0.1";
            }

            // Thêm các tham số theo đúng thứ tự và format VNPay yêu cầu
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddr);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", orderId);

            var paymentUrl = vnpay.CreateRequestUrl(_vnPaySettings.PaymentUrl, _vnPaySettings.HashSecret);
            
            // DEBUG: Log URL để kiểm tra
            Console.WriteLine("=== VNPAY DEBUG ===");
            Console.WriteLine($"Order ID: {orderId}");
            Console.WriteLine($"Amount: {amount} VND = {(long)(amount * 100)} (x100)");
            Console.WriteLine($"TmnCode: {_vnPaySettings.TmnCode}");
            Console.WriteLine($"HashSecret: {_vnPaySettings.HashSecret.Substring(0, 10)}...");
            Console.WriteLine($"ReturnUrl: {_vnPaySettings.ReturnUrl}");
            Console.WriteLine($"IP: {ipAddr}");
            Console.WriteLine($"Full URL: {paymentUrl}");
            Console.WriteLine("===================");
            
            return paymentUrl;
        }

        private async Task<string> CreateMoMoPaymentUrl(string orderId, decimal amount, string orderInfo)
        {
            try
            {
                // Remove Vietnamese characters and special chars from orderInfo for signature
                // MoMo may have issues with UTF-8 in signature calculation
                var cleanOrderInfo = RemoveVietnameseTones(orderInfo);
                
                var paymentUrl = await MoMoLibrary.CreatePaymentUrl(
                    endpoint: _momoSettings.PaymentUrl,
                    partnerCode: _momoSettings.PartnerCode,
                    accessKey: _momoSettings.AccessKey,
                    secretKey: _momoSettings.SecretKey,
                    orderId: orderId,
                    amount: amount,
                    orderInfo: cleanOrderInfo,
                    returnUrl: _momoSettings.ReturnUrl,
                    ipnUrl: _momoSettings.IpnUrl,
                    requestType: _momoSettings.RequestType,
                    extraData: ""
                );

                // DEBUG: Log URL để kiểm tra
                Console.WriteLine("=== MOMO DEBUG ===");
                Console.WriteLine($"Order ID: {orderId}");
                Console.WriteLine($"Amount: {amount} VND");
                Console.WriteLine($"PartnerCode: {_momoSettings.PartnerCode}");
                Console.WriteLine($"ReturnUrl: {_momoSettings.ReturnUrl}");
                Console.WriteLine($"Full URL: {paymentUrl}");
                Console.WriteLine("==================");

                return paymentUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MoMo Error: {ex.Message}");
                throw;
            }
        }

        private string GetIpAddress()
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
                
                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null)
                    {
                        ipAddress = remoteIpAddress.ToString();
                    }
                }
            }
            catch (Exception)
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
        }

        private async Task SendOtpEmailAsync(string toEmail, string customerName, dynamic rentalRecord)
        {
            var subject = $"🔋 Mã OTP #{rentalRecord.Id} - Xác Nhận Thuê Xe EV Rental";
            
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
        .header {{ background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%); color: white; padding: 40px 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; }}
        .content {{ padding: 30px; }}
        .greeting {{ font-size: 18px; color: #1f2937; margin-bottom: 15px; }}
        .otp-section {{ background: linear-gradient(135deg, #eff6ff 0%, #dbeafe 100%); border: 3px dashed #2563eb; padding: 25px; text-align: center; margin: 25px 0; border-radius: 12px; }}
        .otp-label {{ font-size: 14px; color: #6b7280; margin: 0 0 10px 0; font-weight: 500; }}
        .otp-code {{ font-size: 42px; font-weight: bold; color: #2563eb; letter-spacing: 10px; margin: 15px 0; font-family: 'Courier New', monospace; }}
        .otp-expire {{ font-size: 13px; color: #ef4444; margin: 10px 0 0 0; font-weight: 600; }}
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
            <h1>🔋 EV RENTAL SYSTEM</h1>
            <p>Xác Nhận Đơn Thuê Xe Điện</p>
        </div>
        
        <div class='content'>
            <p class='greeting'>Xin chào <strong>{customerName}</strong>,</p>
            <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của <strong>EV Rental</strong>. Đơn thuê xe của bạn đã được tạo thành công! 🎉</p>
            
            <div class='otp-section'>
                <p class='otp-label'>🔐 MÃ OTP XÁC NHẬN</p>
                <div class='otp-code'>{rentalRecord.OtpCode}</div>
                <p class='otp-expire'>⏰ Mã có hiệu lực trong 10 phút kể từ khi nhận email</p>
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
                <p class='cost-label'>Tổng Chi Phí Thanh Toán</p>
                <div class='cost-amount'>{rentalRecord.TotalPrice:N0} VNĐ</div>
                <p style='margin: 0; font-size: 13px; opacity: 0.9;'>Bao gồm: Phí thuê xe + Phí giữ chỗ + Phí cọc</p>
            </div>

            <div class='warning-box'>
                <p style='margin: 0 0 8px 0;'><strong>⚠️ LƯU Ý QUAN TRỌNG</strong></p>
                <ul>
                    <li><strong>Vui lòng mang theo mã OTP</strong> này khi đến nhận xe tại trạm</li>
                    <li>Xuất trình mã OTP cho nhân viên để xác nhận danh tính</li>
                    <li>Chuẩn bị <strong>tiền mặt {rentalRecord.TotalPrice:N0} VNĐ</strong> để thanh toán</li>
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
            <p>Nếu bạn không thực hiện đặt xe này, vui lòng liên hệ ngay: support@evrental.com</p>
            <p style='margin-top: 15px;'>&copy; 2025 EV Rental System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

            await _emailSender.SendAsync(toEmail, subject, htmlBody);
        }

        private string GenerateOtpCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string RemoveVietnameseTones(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            var vietnameseSigns = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
            };

            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                {
                    text = text.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
                }
            }

            return text;
        }
    }
}

