using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class VehicleDto
    {
        public int stationId { get; set; }
        public string name { get; set; } 
        public string brand { get; set; }
        public string plateNumber { get; set; }
        public string model { get; set; }
        public string vehicleType { get; set; }
        public VehicleStatus status { get; set; }
        public decimal pricePerHour { get; set; }
        public decimal pricePerDay { get; set; }
        public string features { get; set; }
        public string imageUrl { get; set; }
        public int maxDistance { get; set; }
        
    }
}
