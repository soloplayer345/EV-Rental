// using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IVehicleRepo : IGenericRepo<Vehicle>
    {
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(
            string? name,
            string? brand,
            VehicleStatus? status
        );
        Task<IEnumerable<Vehicle>> GetAll();
    }
}
