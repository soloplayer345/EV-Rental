using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace EV_Rental.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public LoginRequestDto LoginRequest { get; set; } = new LoginRequestDto();

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

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

            var result = await _authService.LoginAsync(LoginRequest);

            if (!result.Success)
            {
                ErrorMessage = result.Message;
                return Page();
            }

            // Lưu thông tin user vào Session
            var userJson = JsonSerializer.Serialize(result.Data);
            HttpContext.Session.SetString("UserSession", userJson);

            // Redirect dựa theo role
            if (result.Data?.Role == DataAccessLayer.Enums.AccountRole.Admin)
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            else if (result.Data?.Role == DataAccessLayer.Enums.AccountRole.Staff)
            {
                return RedirectToPage("/Staff/Dashboard");
            }
            else
            {
                return RedirectToPage("/Renter/Index");
            }
        }
    }
}
