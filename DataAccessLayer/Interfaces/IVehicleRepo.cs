using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IVehicleRepo : IGenericRepo<Vehicle>
    {
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string? name, string? brand, string? vehicleType, VehicleStatus? status);
    }
}
