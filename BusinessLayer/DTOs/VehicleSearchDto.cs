using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class VehicleSearchDto
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? VehicleType { get; set; }
        public VehicleStatus? Status { get; set; }
    }
}
