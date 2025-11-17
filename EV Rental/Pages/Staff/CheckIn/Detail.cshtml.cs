using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff.CheckIn
{
    public class DetailModel : PageModel
    {
        private readonly ICheckInService _checkInService;

        public DetailModel(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        public RentalRecordDto? RentalRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Kiểm tra authentication
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            if (user?.Role != DataAccessLayer.Entities.AccountRole.Staff)
            {
                return RedirectToPage("/Index");
            }

            try
            {
                RentalRecord = await _checkInService.GetBillingAsync(id);
                
                if (RentalRecord == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy thông tin thuê xe với ID: {id}";
                    return RedirectToPage("./Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostProcessCheckInAsync([FromBody] CheckInDto checkInDto)
        {
            try
            {
                // Lấy thông tin staff từ session
                var user = SessionHelper.GetUserSession(HttpContext.Session);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng đăng nhập" });
                }

                checkInDto.CreatedBy = user.AccountId;

                var result = await _checkInService.ProcessCheckInAsync(checkInDto);

                return new JsonResult(new
                {
                    success = result.Success,
                    message = result.Message,
                    data = new
                    {
                        rentalRecordId = result.RentalRecordId,
                        usageCost = result.UsageCost,
                        lateFee = result.LateFee,
                        totalPenalty = result.TotalPenalty,
                        extraFees = result.ExtraFees,
                        discount = result.Discount,
                        totalAmount = result.TotalAmount,
                        depositFee = result.DepositFee,
                        depositRefund = result.DepositRefund,
                        finalPayment = result.FinalPayment,
                        inspectionProblems = result.InspectionProblems
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = $"Lỗi: {ex.Message}"
                });
            }
        }
    }
}
