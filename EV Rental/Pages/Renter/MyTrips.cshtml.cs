using BusinessLayer.Services;
using DataAccessLayer.Entities;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Renter
{
    public class MyTripsModel : PageModel
    {
        private readonly RentalService _rentalService;
        private readonly VehicleService _vehicleService;

        public MyTripsModel(RentalService rentalService, VehicleService vehicleService)
        {
            _rentalService = rentalService;
            _vehicleService = vehicleService;
        }

        public List<RentalRecord> Rentals { get; set; } = new List<RentalRecord>();
        public Dictionary<int, Vehicle> VehicleDict { get; set; } = new Dictionary<int, Vehicle>();
        public Dictionary<int, Station> PickupStationDict { get; set; } = new Dictionary<int, Station>();
        public Dictionary<int, Station> ReturnStationDict { get; set; } = new Dictionary<int, Station>();

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
            Rentals = await _rentalService.GetRentalsByRenterIdAsync(user.AccountId);

            // Lấy thông tin xe và trạm
            var vehicles = await _vehicleService.GetVehiclesAsync();
            var stations = await _rentalService.GetAllStationsAsync();

            VehicleDict = vehicles.ToDictionary(v => v.Id);

            foreach (var station in stations)
            {
                if (!PickupStationDict.ContainsKey(station.Id))
                    PickupStationDict[station.Id] = new Station { Id = station.Id, Name = station.Name, Address = station.Address, State = station.State };
                if (!ReturnStationDict.ContainsKey(station.Id))
                    ReturnStationDict[station.Id] = new Station { Id = station.Id, Name = station.Name, Address = station.Address, State = station.State };
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
