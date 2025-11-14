using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.RentalRecord
{
    public class IndexModel : PageModel
    {
        private readonly RentalRecordService _rentalRecordService;
        public string UserEmail { get; set; } = string.Empty;
        public List<RentalRecordDto> RentalRecords { get; set; } = new();
        public string SearchQuery { get; set; } = "";
        public string FilterStatus { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";

        public IndexModel(RentalRecordService rentalRecordService)
        {
            _rentalRecordService = rentalRecordService;
        }

        public async Task<IActionResult> OnGetAsync(string search = "", string status = "", string startDate = "", string endDate = "")
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            SearchQuery = search;
            FilterStatus = status;
            StartDate = startDate;
            EndDate = endDate;

            try
            {
                RentalRecordStatus? statusEnum = null;
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<RentalRecordStatus>(status, out var parsedStatus))
                {
                    statusEnum = parsedStatus;
                }

                DateTime? startDateParsed = string.IsNullOrEmpty(startDate) ? null : DateTime.Parse(startDate);
                DateTime? endDateParsed = string.IsNullOrEmpty(endDate) ? null : DateTime.Parse(endDate);

                var records = await _rentalRecordService.SearchRentalRecordsAsync(search, statusEnum, startDateParsed, endDateParsed);
                RentalRecords = records.ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải dữ liệu đơn thuê: {ex.Message}";
            }

            return Page();
        }
    }
}

