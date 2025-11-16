using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Problems
{
    public class IndexModel : PageModel
    {
        private readonly IReportService _reportService;
        private readonly IVehicleService _vehicleService;
        private readonly IAccountService _accountService;
        private readonly IRentalRecordService _rentalRecordService;

        public IndexModel(IReportService reportService, IVehicleService vehicleService, 
            IAccountService accountService, IRentalRecordService rentalRecordService)
        {
            _reportService = reportService;
            _vehicleService = vehicleService;
            _accountService = accountService;
            _rentalRecordService = rentalRecordService;
        }

        public List<BusinessLayer.Services.InspectionProblemReportDto> Problems { get; set; } = new();
        public string? IncidentType { get; set; }
        public string? SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync(string? incidentType = null, string? search = null)
        {
            // Check admin authorization
            if (!EV_Rental.Helpers.SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            IncidentType = incidentType;
            SearchTerm = search;

            Problems = await _reportService.GetAllInspectionProblemsAsync(incidentType);

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(search))
            {
                Problems = Problems.Where(p =>
                    p.RenterName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.VehicleName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetProblemDetailAsync(int id)
        {
            var problems = await _reportService.GetAllInspectionProblemsAsync();
            var problem = problems.FirstOrDefault(p => p.Id == id);

            if (problem == null)
            {
                return Content("<div class='alert alert-danger'>Không tìm thấy báo cáo sự cố</div>");
            }

            var html = $@"
                <div class='row'>
                    <div class='col-md-6'>
                        <p><strong>ID:</strong> #{problem.Id}</p>
                        <p><strong>Đơn thuê:</strong> <a href='/Admin/RentalRecord/Detail/{problem.RentalId}'>#{problem.RentalId}</a></p>
                        <p><strong>Khách hàng:</strong> {problem.RenterName}</p>
                        <p><strong>Xe:</strong> {problem.VehicleName}</p>
                    </div>
                    <div class='col-md-6'>
                        <p><strong>Loại sự cố:</strong> {GetIncidentTypeName(problem.IncidentType)}</p>
                        <p><strong>Phí phạt:</strong> <span class='text-danger fw-bold'>{problem.PenaltyAmount:N0} ₫</span></p>
                        <p><strong>Ngày tạo:</strong> {problem.CreateDate:dd/MM/yyyy HH:mm}</p>
                    </div>
                </div>
                <hr>
                <div>
                    <strong>Mô tả chi tiết:</strong>
                    <p class='mt-2'>{problem.Description}</p>
                </div>
            ";

            return Content(html, "text/html");
        }

        public async Task<IActionResult> OnPostSetMaintenanceAsync(int rentalId)
        {
            try
            {
                var rental = await _rentalRecordService.GetRentalRecordByIdAsync(rentalId);
                if (rental == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đơn thuê" });
                }

                var vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);
                if (vehicle == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy xe" });
                }

                // Update vehicle status to Maintenance
                vehicle.Status = VehicleStatus.Maintenance;
                await _vehicleService.UpdateVehicleAsync(vehicle);

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostBanUserAsync(int rentalId)
        {
            try
            {
                var rental = await _rentalRecordService.GetRentalRecordByIdAsync(rentalId);
                if (rental == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đơn thuê" });
                }

                // Ban the renter account
                await _accountService.ToggleAccountStatusAsync(rental.RenterId, false);

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        private string GetIncidentTypeName(string type)
        {
            return type switch
            {
                "damage" => "Hư hỏng",
                "late_return" => "Trả muộn",
                "no_show" => "Không đến nhận xe",
                "nonpayment" => "Không thanh toán",
                _ => "Khác"
            };
        }
    }
}

