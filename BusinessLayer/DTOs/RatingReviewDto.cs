namespace BusinessLayer.DTOs
{
    public class RatingReviewDto
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // Renter info
        public string RenterName { get; set; } = string.Empty;
        public string RenterEmail { get; set; } = string.Empty;
        
        // Vehicle info
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string VehiclePlateNumber { get; set; } = string.Empty;
        public string VehicleImageUrl { get; set; } = string.Empty;
    }

    public class ReviewStatisticsDto
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public double FiveStarPercentage { get; set; }
        public double FourStarPercentage { get; set; }
        public double ThreeStarPercentage { get; set; }
        public double TwoStarPercentage { get; set; }
        public double OneStarPercentage { get; set; }
    }
}
