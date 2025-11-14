using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class VehicleCreateDto
    {
        public int StationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public VehicleStatus Status { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public string Features { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int MaxDistance { get; set; }
        public int seartCapacity { get; set; }
        public decimal BatteryCapacity { get; set; }
    }
}
