namespace DataAccessLayer.Entities
{
    public class InspectionProblem : BaseEntity
    {
        public int RentalId { get; set; }
        public string IncidentType { get; set; } // 'damage'|'late_return'|'no_show'|'nonpayment'|'other'
        public string Description { get; set; }
        public string Evidence { get; set; } // JSON array of image URLs, video refs
        public decimal PenaltyAmount { get; set; } = 0;
        public int? CreatedBy { get; set; }

        // Navigation properties
        public virtual RentalRecord RentalRecord { get; set; }
    }
}
