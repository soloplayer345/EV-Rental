using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;

        public RegisterModel(IAuthService authService)
        {
            _authService = authService;
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

            var result = await _authService.RegisterAsync(RegisterRequest);

            if (!result.Success)
            {
                ErrorMessage = result.Message;
                if (result.Errors.Any())
                {
                    ErrorMessage += "\n" + string.Join("\n", result.Errors);
                }
                return Page();
            }

            // Đăng ký thành công, redirect về trang login với thông báo
            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToPage("/Account/Login");
        }
    }
}
