using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Renter
{
    public class BillingModel : PageModel
    {
        private readonly ICheckInService _checkinService;

        public BillingModel(ICheckInService checkinService)
        {
            _checkinService = checkinService;
        }
        public void OnGet()
        {
        }

        [BindProperty(SupportsGet = true)]
        public int RentalRecordId { get; set; }

        public RentalRecordDto Bill { get; set; }

        public async Task OnGetAsync(int rentalRecordId)
        {
            Bill = await _checkinService.GetBillingAsync(rentalRecordId);
        }

        public async Task<IActionResult> OnPostPayAsync([FromBody] decimal amount)
        {
            var success = await _checkinService.ConfirmPaymentAsync(RentalRecordId, amount);
            return new JsonResult(new { success, message = success ? "Thanh toán thành công!" : "Thanh toán thất bại!" });
        }
    }
}
