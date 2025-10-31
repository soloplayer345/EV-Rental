using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;

        public RegisterModel(IAuthService authService, IEmailSender emailSender)
        {
            _authService = authService;
            _emailSender = emailSender;
        }

        [BindProperty]
        public RegisterRequestDto RegisterRequest { get; set; } = new RegisterRequestDto();

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Kiểm tra nếu đã đăng nhập thì redirect về trang chủ
            var userSession = HttpContext.Session.GetString("UserSession");
            if (!string.IsNullOrEmpty(userSession))
            {
                Response.Redirect("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // 1) Sinh OTP 6 số
            var rng = new Random();
            var otp = rng.Next(100000, 999999).ToString();

            // 2) Lưu vào Session
            var otpSession = new SessionHelper.RegisterOtpSession
            {
                RegisterRequest = RegisterRequest,
                OtpCode = otp,
                ExpireAtUtc = DateTime.UtcNow.AddMinutes(10),
                ResendCount = 0
            };
            SessionHelper.SetRegisterOtpSession(HttpContext.Session, otpSession);

            // 3) Gửi email
            var subject = "EV Rental - Mã xác thực đăng ký";
            var body = $@"<p>Xin chào,</p>
                          <p>Mã xác thực đăng ký của bạn là: <strong style='font-size:20px'>{otp}</strong></p>
                          <p>Mã có hiệu lực trong 10 phút.</p>
                          <p>Trân trọng,<br/>EV Rental</p>";
            try
            {
                await _emailSender.SendAsync(RegisterRequest.Email, subject, body);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Không thể gửi email OTP. Vui lòng kiểm tra cấu hình SMTP hoặc thử lại sau.";
                return Page();
            }

            TempData["ShowOtpModal"] = "true";
            return Page();
        }

        public async Task<IActionResult> OnPostVerifyOtpAsync(string otp)
        {
            var data = SessionHelper.GetRegisterOtpSession(HttpContext.Session);
            if (data == null)
            {
                ErrorMessage = "Phiên xác thực đã hết hạn. Vui lòng đăng ký lại.";
                return Page();
            }

            RegisterRequest = data.RegisterRequest;

            if (DateTime.UtcNow > data.ExpireAtUtc)
            {
                ErrorMessage = "Mã OTP đã hết hạn. Vui lòng gửi lại mã.";
                TempData["ShowOtpModal"] = "true";
                return Page();
            }

            if (!string.Equals(data.OtpCode, otp))
            {
                ErrorMessage = "Mã OTP không đúng. Vui lòng thử lại.";
                TempData["ShowOtpModal"] = "true";
                return Page();
            }

            // OTP đúng -> Tạo tài khoản
            var result = await _authService.RegisterAsync(data.RegisterRequest);
            if (!result.Success)
            {
                ErrorMessage = result.Message;
                if (result.Errors.Any())
                {
                    ErrorMessage += "\n" + string.Join("\n", result.Errors);
                }
                TempData["ShowOtpModal"] = "true";
                return Page();
            }

            SessionHelper.ClearRegisterOtp(HttpContext.Session);
            TempData["SuccessMessage"] = "Xác thực thành công! Vui lòng đăng nhập.";
            return RedirectToPage("/Account/Login");
        }

        public async Task<IActionResult> OnPostResendOtpAsync()
        {
            var data = SessionHelper.GetRegisterOtpSession(HttpContext.Session);
            if (data == null)
            {
                ErrorMessage = "Phiên xác thực đã hết hạn. Vui lòng đăng ký lại.";
                return Page();
            }

            RegisterRequest = data.RegisterRequest;

            if (data.ResendCount >= 3)
            {
                ErrorMessage = "Bạn đã yêu cầu gửi lại quá số lần cho phép. Vui lòng đăng ký lại.";
                return Page();
            }

            var rng = new Random();
            data.OtpCode = rng.Next(100000, 999999).ToString();
            data.ExpireAtUtc = DateTime.UtcNow.AddMinutes(10);
            data.ResendCount += 1;
            SessionHelper.SetRegisterOtpSession(HttpContext.Session, data);

            var subject = "EV Rental - Mã xác thực đăng ký (gửi lại)";
            var body = $@"<p>Mã OTP mới của bạn: <strong style='font-size:20px'>{data.OtpCode}</strong></p>
                          <p>Mã có hiệu lực trong 10 phút.</p>";
            await _emailSender.SendAsync(data.RegisterRequest.Email, subject, body);

            TempData["ShowOtpModal"] = "true";
            return Page();
        }
    }
}
