namespace DataAccessLayer.Entities
{
    public class Report : BaseEntity
    {
        public int StationId { get; set; }

        public int CreatedBy { get; set; }
        public string ReportType { get; set; }
        public string Content { get; set; }

        // Navigation properties
        public virtual Station Station { get; set; }
    }
}
