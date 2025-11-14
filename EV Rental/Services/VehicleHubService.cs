using BusinessLayer.Services;
using DataAccessLayer.Entities;
using EV_Rental.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EV_Rental.Services
{
    public class VehicleHubService
    {
        private readonly VehicleService _vehicleService;
        private readonly IHubContext<VehicleHub> _hubContext;

        public VehicleHubService(VehicleService vehicleService, IHubContext<VehicleHub> hubContext)
        {
            _vehicleService = vehicleService;
            _hubContext = hubContext;
        }

        public async Task AddVehicleWithNotificationAsync(Vehicle vehicle)
        {
            await _vehicleService.AddVehicleAsync(vehicle);
            await _hubContext.Clients.All.SendAsync("ReceiveVehicleUpdate", "added", vehicle);
        }

        public async Task UpdateVehicleWithNotificationAsync(Vehicle vehicle)
        {
            await _vehicleService.UpdateVehicleAsync(vehicle);
            await _hubContext.Clients.All.SendAsync("ReceiveVehicleUpdate", "updated", vehicle);
        }

        public async Task DeleteVehicleWithNotificationAsync(int vehicleId)
        {
            await _vehicleService.DeleteVehicleAsync(vehicleId);
            await _hubContext.Clients.All.SendAsync("ReceiveVehicleUpdate", "deleted", new { Id = vehicleId });
        }
    }
}
