using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs
{
    public class CreateRatingReviewDto
    {
        public int RentalId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn số sao đánh giá")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        public int Rating { get; set; }
        
        [StringLength(1000, ErrorMessage = "Nhận xét không được quá 1000 ký tự")]
        public string? Comment { get; set; }
    }
}
