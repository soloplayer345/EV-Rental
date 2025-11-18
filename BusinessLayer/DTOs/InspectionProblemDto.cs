using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs
{
    public class InspectionProblemDto
    {
        public int Id { get; set; }
        
        public int RentalId { get; set; }
        
        [Required(ErrorMessage = "Loại sự cố là bắt buộc")]
        public string IncidentType { get; set; } = string.Empty; // 'damage'|'missing_items'|'dirty'|'fuel_shortage'|'other'
        
        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; } = string.Empty;
        
        public string Evidence { get; set; } = string.Empty; // JSON array of image URLs
        
        public string? ImageUrl { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền phạt phải lớn hơn hoặc bằng 0")]
        public decimal PenaltyAmount { get; set; } = 0;
        
        public int? CreatedBy { get; set; }
        
        public DateTime CreateDate { get; set; }
    }
}
