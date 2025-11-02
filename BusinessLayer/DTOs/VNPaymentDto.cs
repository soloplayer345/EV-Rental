namespace BusinessLayer.DTOs
{
    public class VNPaymentRequestDto
    {
        public int RentalId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; } = string.Empty;
        public string OrderType { get; set; } = "rental";
        public string Locale { get; set; } = "vn";
    }

    public class VNPaymentResponseDto
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string VnPayResponseCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
