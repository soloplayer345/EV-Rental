using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace EV_Rental.Pages.Renter
{
    public class PaymentModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly IRentalService _rentalService;
        private readonly IPaymentService _paymentService;
        private readonly IAccountService _accountService;
        private readonly VNPaySettings _vnPaySettings;
        private readonly MoMoSettings _momoSettings;
        private readonly IEmailSender _emailSender;

        public PaymentModel(
            IVehicleService vehicleService,
            IRentalService rentalService,
            IPaymentService paymentService,
            IAccountService accountService,
            IOptions<VNPaySettings> vnPaySettings,
            IOptions<MoMoSettings> momoSettings,
            IEmailSender emailSender
        )
        {
            _vehicleService = vehicleService;
            _rentalService = rentalService;
            _paymentService = paymentService;
            _accountService = accountService;
            _vnPaySettings = vnPaySettings.Value;
            _momoSettings = momoSettings.Value;
            _emailSender = emailSender;
        }

        public RentalRecordDto RentalRecord { get; set; } = new RentalRecordDto();
        public VehicleDto? Vehicle { get; set; }
        public string PickupStationName { get; set; } = string.Empty;
        public string ReturnStationName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public int RentalId { get; set; }

        [BindProperty]
        public string PaymentMethod { get; set; } = "momo";

        public async Task<IActionResult> OnGetAsync(int rentalId)
        {
            // Check if user is logged in
            var accountId = SessionHelper.GetAccountId(HttpContext);
            if (accountId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get rental record
            var rentalEntities = await _rentalService.GetRentalsByRenterIdAsync(accountId.Value);
            var rental = rentalEntities.FirstOrDefault(r => r.Id == rentalId);

            if (rental == null)
            {
                ErrorMessage = "Không tìm thấy đơn thuê.";
                return Page();
            }

            // Check if rental belongs to current user
            if (rental.RenterId != accountId.Value)
            {
                ErrorMessage = "Bạn không có quyền truy cập đơn thuê này.";
                return Page();
            }

            // Check if rental needs payment (Pending OR Completed with ExtraFees)
            bool needsPayment =
                rental.Status == RentalRecordStatus.Pending
                || (rental.Status == RentalRecordStatus.Completed && rental.ExtraFees > 0);

            if (!needsPayment)
            {
                ErrorMessage = "Đơn thuê này không cần thanh toán.";
                return Page();
            }

            RentalRecord = BusinessLayer.Mapping.RentalRecordMapper.ToDto(rental);

            // Load vehicle
            Vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);

            // Load station names
            var stations = await _rentalService.GetAllStationsAsync();
            PickupStationName =
                stations.FirstOrDefault(s => s.Id == rental.PickupStationId)?.Name ?? "N/A";
            ReturnStationName =
                rental.ReturnStationId.HasValue
                && stations.Any(s => s.Id == rental.ReturnStationId.Value)
                    ? stations.First(s => s.Id == rental.ReturnStationId.Value).Name
                    : "N/A";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if user is logged in
            var accountId = SessionHelper.GetAccountId(HttpContext);
            if (accountId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get rental record
            var rentalEntities = await _rentalService.GetRentalsByRenterIdAsync(accountId.Value);
            var rental = rentalEntities.FirstOrDefault(r => r.Id == RentalId);

            if (rental == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn thuê.";
                return RedirectToPage("/Renter/MyTrips");
            }

            // Check if rental belongs to current user
            if (rental.RenterId != accountId.Value)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập đơn thuê này.";
                return RedirectToPage("/Renter/MyTrips");
            }

            // Check if rental needs payment (Pending OR Completed with ExtraFees)
            bool needsPayment =
                rental.Status == RentalRecordStatus.Pending
                || (rental.Status == RentalRecordStatus.Completed && rental.ExtraFees > 0);

            if (!needsPayment)
            {
                TempData["ErrorMessage"] = "Đơn thuê này không cần thanh toán.";
                return RedirectToPage("/Renter/MyTrips");
            }

            // Determine amount to pay (full amount for Pending, only ExtraFees for Completed)
            decimal totalAmount;
            string paymentDescription;

            if (rental.Status == RentalRecordStatus.Pending)
            {
                totalAmount = rental.TotalPrice;
                paymentDescription = "Thanh toan don thue xe";
            }
            else // Completed with ExtraFees
            {
                totalAmount = rental.ExtraFees;
                paymentDescription = "Thanh toan phi phat";
            }

            var transactionRef = $"PAYMENT{RentalId}_{DateTime.Now:yyyyMMddHHmmss}";

            // Load vehicle for payment description
            var vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);
            var vehicleName = vehicle?.Name ?? "xe";

            // Store rental ID in session
            HttpContext.Session.SetString($"PendingRentalId_{transactionRef}", RentalId.ToString());

            // Process based on payment method
            if (PaymentMethod == "momo")
            {
                try
                {
                    var momoUrl = await CreateMoMoPaymentUrl(
                        transactionRef,
                        totalAmount,
                        $"{paymentDescription} {vehicleName} - Ma don {RentalId}"
                    );
                    return Redirect(momoUrl);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Không thể tạo thanh toán MoMo: {ex.Message}";

                    // Reload data for display
                    RentalRecord = BusinessLayer.Mapping.RentalRecordMapper.ToDto(rental);
                    Vehicle = vehicle;
                    var stations = await _rentalService.GetAllStationsAsync();
                    PickupStationName =
                        stations.FirstOrDefault(s => s.Id == rental.PickupStationId)?.Name ?? "N/A";
                    ReturnStationName =
                        rental.ReturnStationId.HasValue
                        && stations.Any(s => s.Id == rental.ReturnStationId.Value)
                            ? stations.First(s => s.Id == rental.ReturnStationId.Value).Name
                            : "N/A";

                    return Page();
                }
            }
            else if (PaymentMethod == "vnpay")
            {
                var vnpayUrl = CreateVNPayPaymentUrl(
                    transactionRef,
                    totalAmount,
                    $"{paymentDescription} {vehicleName} - Ma don {RentalId}"
                );
                return Redirect(vnpayUrl);
            }
            else
            {
                ErrorMessage = "Phương thức thanh toán không hợp lệ.";

                // Reload data for display
                RentalRecord = BusinessLayer.Mapping.RentalRecordMapper.ToDto(rental);
                Vehicle = vehicle;
                var stations = await _rentalService.GetAllStationsAsync();
                PickupStationName =
                    stations.FirstOrDefault(s => s.Id == rental.PickupStationId)?.Name ?? "N/A";
                ReturnStationName =
                    rental.ReturnStationId.HasValue
                    && stations.Any(s => s.Id == rental.ReturnStationId.Value)
                        ? stations.First(s => s.Id == rental.ReturnStationId.Value).Name
                        : "N/A";

                return Page();
            }
        }

        private string CreateVNPayPaymentUrl(string orderId, decimal amount, string orderInfo)
        {
            var vnpay = new VNPayLibrary();

            // Get IP address
            string ipAddr = GetIpAddress();
            if (string.IsNullOrEmpty(ipAddr))
            {
                ipAddr = "127.0.0.1";
            }

            // Add request data
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

            var paymentUrl = vnpay.CreateRequestUrl(
                _vnPaySettings.PaymentUrl,
                _vnPaySettings.HashSecret
            );

            return paymentUrl;
        }

        private async Task<string> CreateMoMoPaymentUrl(
            string orderId,
            decimal amount,
            string orderInfo
        )
        {
            // Remove Vietnamese characters for MoMo compatibility
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
                    if (
                        remoteIpAddress.AddressFamily
                        == System.Net.Sockets.AddressFamily.InterNetworkV6
                    )
                    {
                        remoteIpAddress = System
                            .Net.Dns.GetHostEntry(remoteIpAddress)
                            .AddressList.FirstOrDefault(x =>
                                x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                            );
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

        private string RemoveVietnameseTones(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

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
                "ÝỲỴỶỸ",
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
