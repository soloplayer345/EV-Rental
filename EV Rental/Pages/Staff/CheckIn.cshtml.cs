using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class CheckInModel : PageModel
    {
        private readonly CheckInService _checkInService;

        public CheckInModel(CheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [BindProperty(SupportsGet = true)]
        public int RentalRecordId { get; set; }

        public RentalRecordDto Billing { get; set; }

        public void OnGet()
        {
        }

        public async Task OnGetAsync(int rentalRecordId)
        {
            Billing = await _checkInService.GetBillingAsync(rentalRecordId);
        }

        public async Task<IActionResult> OnPostProcessAsync([FromBody] CheckInDto checkInDto)
        {
            var success = await _checkInService.ProcessCheckInAsync(checkInDto);
            return new JsonResult(new { success, message = success ? "Check-in thành công" : "Lỗi khi check-in" });
        }
    }
}
