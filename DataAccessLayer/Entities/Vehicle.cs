using DataAccessLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Vehicle : BaseEntity
    {
        public int StationId { get; set; }
        public string? Name { get; set; } // Tên xe (VD: VinFast VF7 Plus 2025)
        public string? Brand { get; set; }
        public string? PlateNumber { get; set; } // Biển số xe
        public string? Model { get; set; }
        public string? VehicleType { get; set; } // 'scooter'|'motorbike'|'car'...
        public VehicleStatus Status { get; set; } = VehicleStatus.Available; // 'available'|'waitingForPickup'|'rented'|'maintenance'|'charging'
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public string? Features { get; set; } // e.g. {"gps":true,"insurance":true}
        public string? ImageUrl { get; set; }
        public int MaxDistance { get; set; } // Quãng đường tối đa (km)
        public int seartCapacity { get; set; } // Sức chứa chỗ ngồi
        public decimal BatteryCapacity { get; set; } // Dung lượng pin (kWh)

        // Navigation properties
        [ScaffoldColumn(false)]
        public virtual Station? Station { get; set; }
        [ScaffoldColumn(false)]
        public virtual ICollection<RentalRecord> RentalRecords { get; set; } = new List<RentalRecord>();
    }
}
