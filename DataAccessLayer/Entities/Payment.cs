namespace DataAccessLayer.Entities
{
    public class Payment : BaseEntity
    {
        public int RentalId { get; set; }
        public decimal Amount { get; set; } // Tổng số tiền thanh toán
        public string Method { get; set; } // 'cash'|'card'|'e-wallet'|'bank-transfer'
        public string TransactionRef { get; set; }
        public string Status { get; set; } = "pending"; // 'pending'|'paid'|'failed'|'refunded'
        public DateTime? PaidAt { get; set; }

        // Navigation properties
        public virtual RentalRecord RentalRecord { get; set; }
    }
}
