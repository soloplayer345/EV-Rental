namespace BusinessLayer.DTOs
{
    public class CreateRentalRequestDto
    {
        public int VehicleId { get; set; }
        public int PickupStationId { get; set; }
        public int ReturnStationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ExpectedEndTime { get; set; }
        public string? Notes { get; set; }
    }
}
