using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using BusinessLayer.Services;
using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Renter
{
    public class ConfirmRentalModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private readonly RentalService _rentalService;
        private readonly PaymentService _paymentService;
        private readonly VNPaySettings _vnPaySettings;

        public ConfirmRentalModel(
            VehicleService vehicleService, 
            RentalService rentalService,
            PaymentService paymentService,
            IOptions<VNPaySettings> vnPaySettings)
        {
            _vehicleService = vehicleService;
            _rentalService = rentalService;
            _paymentService = paymentService;
            _vnPaySettings = vnPaySettings.Value;
        }

        public Vehicle? Vehicle { get; set; }
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

            // Generate transaction reference
            var transactionRef = $"BOOKING{accountId.Value}_{DateTime.Now:yyyyMMddHHmmss}";

            // Store booking info in session for later use (when payment succeeds)
            HttpContext.Session.SetString($"BookingData_{transactionRef}", System.Text.Json.JsonSerializer.Serialize(bookingDto));

            // Redirect to VNPay
            var vnpayUrl = CreateVNPayPaymentUrl(transactionRef, TotalCost, $"Thanh toan thue xe {Vehicle?.Name}");
            return Redirect(vnpayUrl);
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
    }
}
