using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class CheckInDto
    {
        public int RentalRecordId { get; set; }
        public DateTime ActualReturnTime { get; set; }
        public decimal ExtraFees { get; set; }
        public decimal Discount {  get; set; }
        public bool HasDamage { get; set; }
        public string? DamageDescription { get; set; }
        public decimal DamageCost { get; set; }
    }
}
