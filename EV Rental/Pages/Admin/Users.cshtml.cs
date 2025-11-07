using BusinessLayer.DTOs;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using AccountEntity = DataAccessLayer.Entities.Account;

namespace EV_Rental.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Properties for display
        public List<AccountEntity> Users { get; set; } = new List<AccountEntity>();
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

            // Get all users except Admins
            var allUsers = (await _unitOfWork.AccountRepo.GetAllAsync())
                .Where(u => u.Role != AccountRole.Admin)
                .ToList();

            // Apply filters
            Users = allUsers;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Users = Users.Where(u =>
                    u.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Phone.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (RoleFilter.HasValue)
            {
                Users = Users.Where(u => (int)u.Role == RoleFilter.Value).ToList();
            }

            if (StatusFilter.HasValue)
            {
                Users = Users.Where(u => u.IsActive == StatusFilter.Value).ToList();
            }

            // Order by creation date (newest first)
            Users = Users.OrderByDescending(u => u.CreateDate).ToList();

            // Calculate statistics
            TotalUsers = allUsers.Count;
            ActiveUsers = allUsers.Count(u => u.IsActive);
            InactiveUsers = allUsers.Count(u => !u.IsActive);
            
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            NewUsersThisMonth = allUsers.Count(u => u.CreateDate >= firstDayOfMonth);

            return Page();
        }

        public async Task<IActionResult> OnGetUserDetailsAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.AccountRepo.GetByIdAsync(userId);
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
                        updatedAt = user.UpdateDate ?? user.CreateDate,
                        rentalRecords = user.RentalRecords
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

                // Check if email or phone already exists
                if (await _unitOfWork.AccountRepo.IsEmailExistsAsync(Email))
                {
                    TempData["ErrorMessage"] = "Email đã tồn tại";
                    return RedirectToPage();
                }

                if (await _unitOfWork.AccountRepo.IsPhoneExistsAsync(Phone))
                {
                    TempData["ErrorMessage"] = "Số điện thoại đã tồn tại";
                    return RedirectToPage();
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

                // Create new user
                var newUser = new AccountEntity
                {
                    FullName = FullName,
                    Email = Email,
                    Phone = Phone,
                    PasswordHash = passwordHash,
                    Role = (AccountRole)Role,
                    IsActive = IsActive,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                await _unitOfWork.AccountRepo.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm user mới thành công";
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
                var user = await _unitOfWork.AccountRepo.GetByIdAsync(UserId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy user";
                    return RedirectToPage();
                }

                // Check if email is changed and already exists
                if (user.Email != Email && await _unitOfWork.AccountRepo.IsEmailExistsAsync(Email))
                {
                    TempData["ErrorMessage"] = "Email đã tồn tại";
                    return RedirectToPage();
                }

                // Check if phone is changed and already exists
                if (user.Phone != Phone && await _unitOfWork.AccountRepo.IsPhoneExistsAsync(Phone))
                {
                    TempData["ErrorMessage"] = "Số điện thoại đã tồn tại";
                    return RedirectToPage();
                }

                // Update user
                user.FullName = FullName;
                user.Email = Email;
                user.Phone = Phone;
                user.Role = (AccountRole)Role;
                user.IsActive = IsActive;
                user.UpdateDate = DateTime.Now;

                await _unitOfWork.AccountRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật user thành công";
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
                var user = await _unitOfWork.AccountRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy user" });
                }

                user.IsActive = isActive;
                user.UpdateDate = DateTime.Now;

                await _unitOfWork.AccountRepo.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Cập nhật trạng thái thành công" });
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
                var user = await _unitOfWork.AccountRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy user" });
                }

                // Hard delete since IsDeleted is not in database
                await _unitOfWork.AccountRepo.Delete(user);
                await _unitOfWork.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Xóa user thành công" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
