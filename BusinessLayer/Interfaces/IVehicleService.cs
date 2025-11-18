using BusinessLayer.DTOs;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> SearchVehiclesAsync(string name, string brand, VehicleStatus status);
        Task<IEnumerable<VehicleDto>> GetVehiclesAsync();
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<IEnumerable<VehicleDto>> GetVehiclesByStationIdAsync(int stationId);
        Task<VehicleDto?> GetVehicleByIdAsync(int id);
        Task AddVehicleAsync(VehicleCreateDto vehicleDto);
        Task UpdateVehicleAsync(VehicleUpdateDto vehicleDto);
        Task UpdateVehicleAsync(VehicleDto vehicleDto);
        Task DeleteVehicleAsync(int id);
    }
}
