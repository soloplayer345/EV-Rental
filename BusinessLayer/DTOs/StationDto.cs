using DataAccessLayer.Entities;
using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    public class StationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        
        // Navigation property
        public ICollection<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();
    }
}
