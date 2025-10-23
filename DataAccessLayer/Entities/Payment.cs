namespace DataAccessLayer.Entities
{
    public class Payment : BaseEntity
    {
        public int RentalId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Deposit { get; set; } // Số tiền đặt cọc trước khi thuê xe
        public decimal Refund { get; set; } // Số tiền hoàn trả lại cho khách

        // Navigation properties
        public virtual RentalRecord RentalRecord { get; set; }
    }
}
