namespace BusinessLayer.DTOs
{
    public class CancellationPolicyDto
    {
        public int RentalId { get; set; }
        public bool CanCancel { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal BasePrice { get; set; }
        public decimal ReservationFee { get; set; }
        public decimal DepositFee { get; set; }
        public string PolicyText { get; set; }
        public string RefundPercentage { get; set; } // "100%", "50%", "0%"
        public DateTime StartTime { get; set; }
        public TimeSpan TimeUntilStart { get; set; }
    }
}
