using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using BusinessLayer.Mapping;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Renter
{
    public class MyTripsModel : PageModel
    {
        private readonly IRentalService _rentalService;
        private readonly IVehicleService _vehicleService;

        public MyTripsModel(IRentalService rentalService, IVehicleService vehicleService)
        {
            _rentalService = rentalService;
            _vehicleService = vehicleService;
        }

        public List<RentalRecordDto> Rentals { get; set; } = new List<RentalRecordDto>();
        public Dictionary<int, VehicleDto> VehicleDict { get; set; } = new Dictionary<int, VehicleDto>();
        public Dictionary<int, StationDto> PickupStationDict { get; set; } = new Dictionary<int, StationDto>();
        public Dictionary<int, StationDto> ReturnStationDict { get; set; } = new Dictionary<int, StationDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Lấy danh sách rental của user
            var rentalEntities = await _rentalService.GetRentalsByRenterIdAsync(user.AccountId);
            Rentals = RentalRecordMapper.ToDtoList(rentalEntities);

            // Lấy thông tin xe và trạm
            var vehicles = await _vehicleService.GetVehiclesAsync();
            var stations = await _rentalService.GetAllStationsAsync();

            VehicleDict = vehicles.ToDictionary(v => v.Id);

            foreach (var station in stations)
            {
                if (!PickupStationDict.ContainsKey(station.Id))
                    PickupStationDict[station.Id] = station;
                if (!ReturnStationDict.ContainsKey(station.Id))
                    ReturnStationDict[station.Id] = station;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCancelRentalAsync(int rentalId)
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Hủy đơn thuê
            var result = await _rentalService.CancelRentalAsync(rentalId, user.AccountId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToPage();
        }
    }
}

