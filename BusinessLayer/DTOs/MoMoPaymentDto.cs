namespace BusinessLayer.DTOs
{
    public class MoMoPaymentDto
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string IpnUrl { get; set; } = string.Empty;
        public string ExtraData { get; set; } = string.Empty;
        public string RequestType { get; set; } = "captureWallet";
    }
}
