namespace BusinessLayer.DTOs
{
    public class PendingBookingDto
    {
        public int AccountId { get; set; }
        public int VehicleId { get; set; }
        public int PickupStationId { get; set; }
        public int ReturnStationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ExpectedEndTime { get; set; }
        public decimal BasePrice { get; set; }
        public decimal ReservationFee { get; set; }
        public decimal DepositFee { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
    }
}
