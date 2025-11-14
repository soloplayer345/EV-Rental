namespace BusinessLayer.DTOs
{
    public class CheckInResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RentalRecordId { get; set; }
        public decimal UsageCost { get; set; }
        public decimal LateFee { get; set; }
        public decimal TotalPenalty { get; set; }
        public decimal ExtraFees { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DepositFee { get; set; }
        public decimal DepositRefund { get; set; }
        public decimal FinalPayment { get; set; }
        public List<InspectionProblemDto> InspectionProblems { get; set; } = new();
    }
}
