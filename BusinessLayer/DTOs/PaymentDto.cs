using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class PaymentDto
    {
        public int RentalRecordId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
    }
}
