using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class VehicleRepo : GenericRepo<Vehicle>, IVehicleRepo
    {
        public VehicleRepo(EVRentalDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string? name, string? brand, string? vehicleType, VehicleStatus? status)
        {
            IQueryable<Vehicle> query = _dbSet.Where(x => !x.IsDeleted);
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(v => v.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(v => v.Brand.Contains(brand));
            }

            if (!string.IsNullOrEmpty(vehicleType))
            {
                query = query.Where(v => v.VehicleType.Equals(vehicleType, StringComparison.OrdinalIgnoreCase));
            }

            if (status.HasValue && Enum.IsDefined(typeof(VehicleStatus), status.Value))
            {
                query = query.Where(v => v.Status == status);
            }

            return await query.ToListAsync();
        }

        // Override GetByIdAsync to include Station
        public new async Task<Vehicle> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than 0", nameof(id));

            Vehicle? vehicle = await _dbSet
                .Include(v => v.Station)
                .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
            
            if (vehicle == null)
            {
                throw new KeyNotFoundException($"Vehicle with ID {id} not found");
            }
            
            return vehicle;
        }
    }
}
