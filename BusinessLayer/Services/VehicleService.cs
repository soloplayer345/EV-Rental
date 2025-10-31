using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class VehicleService
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IUnitOfWork _unitOfWork;
        public VehicleService(IVehicleRepo vehicleRepo, IUnitOfWork unitOfWork)
        {
            _vehicleRepo = vehicleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DataAccessLayer.Entities.Vehicle>> SearchVehiclesAsync(string name, string brand, string vehicleType, DataAccessLayer.Enums.VehicleStatus status)
        {
            return await _vehicleRepo.SearchVehiclesAsync(name, brand, vehicleType, status);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync()
        {
            return await _vehicleRepo.GetAllAsync();
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _vehicleRepo.GetByIdAsync(id);
        }

        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await _vehicleRepo.AddAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _vehicleRepo.Update(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (vehicle != null)
            {
                _vehicleRepo.Delete(vehicle);
                await _unitOfWork.SaveChangesAsync();
            }
        }

    }
}
