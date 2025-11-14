using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class CheckInResultDto
    {
        public int RentalRecordId { get; set; }
        public decimal TotalHours { get; set; }
        public decimal TotalAmount { get; set; }
        public string Message { get; set; }
    }
}
