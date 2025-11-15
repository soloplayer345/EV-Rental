namespace BusinessLayer.DTOs
{
    public class DashboardDto
    {
        public int TotalAccounts { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalRentals { get; set; }
        public decimal TotalRevenue { get; set; }
        // Thêm các trường khác nếu cần
    }
}