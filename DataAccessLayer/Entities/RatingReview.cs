namespace DataAccessLayer.Entities
{
    public class RatingReview : BaseEntity
    {
        public int RentalId { get; set; }
        public int Rating { get; set; } // 1..5
        public string Comment { get; set; }

        // Navigation properties
        public virtual RentalRecord RentalRecord { get; set; }
    }
}
