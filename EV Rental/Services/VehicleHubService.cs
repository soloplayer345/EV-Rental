using BusinessLayer.Services;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using BusinessLayer.Mapping;
using DataAccessLayer.Entities;
using EV_Rental.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EV_Rental.Services
{
    public class VehicleHubService
    {
        private readonly IVehicleService _vehicleService;
        private readonly IHubContext<VehicleHub> _hubContext;

        public VehicleHubService(IVehicleService vehicleService, IHubContext<VehicleHub> hubContext)
        {
            _vehicleService = vehicleService;
            _hubContext = hubContext;
        }

        public async Task AddVehicleWithNotificationAsync(VehicleCreateDto vehicleDto)
        {
            await _vehicleService.AddVehicleAsync(vehicleDto);
            var vehicle = VehicleMapper.ToVehicleEntity(vehicleDto);
            await _hubContext.Clients.All.SendAsync("ReceiveVehicleUpdate", "added", vehicle);
        }

        public async Task UpdateVehicleWithNotificationAsync(VehicleUpdateDto vehicleDto)
        {
            await _vehicleService.UpdateVehicleAsync(vehicleDto);
            // Get the updated vehicle from service
            var vehicleData = await _vehicleService.GetVehicleByIdAsync(vehicleDto.Id);
            await _hubContext.Clients.All.SendAsync("ReceiveVehicleUpdate", "updated", vehicleData);
        }

        public async Task DeleteVehicleWithNotificationAsync(int vehicleId)
        {
            await _vehicleService.DeleteVehicleAsync(vehicleId);
            await _hubContext.Clients.All.SendAsync("ReceiveVehicleUpdate", "deleted", new { Id = vehicleId });
        }
    }
}
