using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs
{
    public class CheckInDto
    {
        [Required(ErrorMessage = "Rental Record ID là bắt buộc")]
        public int RentalRecordId { get; set; }
        
        [Required(ErrorMessage = "Thời gian trả xe thực tế là bắt buộc")]
        public DateTime ActualReturnTime { get; set; }
        
        public int? ReturnStationId { get; set; }
        
        public decimal ExtraFees { get; set; } = 0;
        
        public decimal Discount { get; set; } = 0;
        
        public string? Notes { get; set; }
        
        public List<InspectionProblemDto> InspectionProblems { get; set; } = new();
        
        public int CreatedBy { get; set; }
    }
}
