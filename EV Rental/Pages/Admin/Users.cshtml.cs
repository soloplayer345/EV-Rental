using BusinessLayer.Services;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.DTOs;

namespace EV_Rental.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly AccountService _accountService;

        public UsersModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        // Properties for display
        public List<AccountDto> Users { get; set; } = new List<AccountDto>();
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? RoleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? StatusFilter { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is admin
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var currentUser = SessionHelper.GetUserSession(HttpContext.Session);
            if (currentUser?.Role != AccountRole.Admin)
            {
                return RedirectToPage("/Index");
            }

            // Get all users using service
            var searchResult = await _accountService.SearchAccountsAsync(
                SearchTerm, 
                RoleFilter.HasValue ? (AccountRole)RoleFilter.Value : null, 
                StatusFilter
            );
            Users = searchResult.ToList();

            // Calculate statistics
            var stats = await _accountService.GetUserStatisticsAsync();
            TotalUsers = stats.TotalUsers;
            ActiveUsers = stats.ActiveUsers;
            InactiveUsers = stats.InactiveUsers;
            NewUsersThisMonth = stats.NewUsersThisMonth;

            return Page();
        }

        public async Task<IActionResult> OnGetUserDetailsAsync(int userId)
        {
            try
            {
                var user = await _accountService.GetAccountByIdAsync(userId);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy user" });
                }

                return new JsonResult(new
                {
                    success = true,
                    user = new
                    {
                        id = user.Id,
                        fullName = user.FullName,
                        email = user.Email,
                        phone = user.Phone,
                        role = (int)user.Role,
                        isActive = user.IsActive,
                        createdAt = user.CreateDate,
                        updatedAt = user.UpdateDate ?? user.CreateDate
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostAddUserAsync([FromForm] string FullName, [FromForm] string Email, 
            [FromForm] string Phone, [FromForm] int Role, [FromForm] string Password, 
            [FromForm] string ConfirmPassword, [FromForm] bool IsActive)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || 
                    string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Password))
                {
                    TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin";
                    return RedirectToPage();
                }

                if (Password != ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "Mật khẩu không khớp";
                    return RedirectToPage();
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

                // Create new user using service
                // Admin tạo user sẽ tự động active, không cần chờ duyệt
                var newUser = new
                {
                    FullName = FullName,
                    Email = Email,
                    Phone = Phone,
                    PasswordHash = passwordHash,
                    Role = (AccountRole)Role,
                    IsActive = true // Admin thêm user tự động active
                };

                await _accountService.AddAccountAsync((dynamic)newUser);

                TempData["SuccessMessage"] = "Thêm user mới thành công";
                return RedirectToPage();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostEditUserAsync([FromForm] int UserId, [FromForm] string FullName, 
            [FromForm] string Email, [FromForm] string Phone, [FromForm] int Role, [FromForm] bool IsActive)
        {
            try
            {
                var user = await _accountService.GetAccountByIdAsync(UserId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy user";
                    return RedirectToPage();
                }

                // Update user info - convert DTO to entity for update
                var accountEntity = new DataAccessLayer.Entities.Account
                {
                    Id = user.Id,
                    FullName = FullName,
                    Email = Email,
                    Phone = Phone,
                    PasswordHash = "", // Keep existing password
                    Role = (AccountRole)Role,
                    IsActive = IsActive,
                    CreateDate = user.CreateDate,
                    UpdateDate = DateTime.Now
                };

                await _accountService.UpdateAccountAsync(accountEntity);

                TempData["SuccessMessage"] = "Cập nhật user thành công";
                return RedirectToPage();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage();
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostToggleStatusAsync([FromForm] int userId, [FromForm] bool isActive)
        {
            try
            {
                await _accountService.ToggleAccountStatusAsync(userId, isActive);
                return new JsonResult(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDeleteUserAsync([FromForm] int userId)
        {
            try
            {
                await _accountService.DeleteAccountAsync(userId);
                return new JsonResult(new { success = true, message = "Xóa user thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

