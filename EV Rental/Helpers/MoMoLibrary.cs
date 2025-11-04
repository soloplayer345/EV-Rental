using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EV_Rental.Helpers
{
    public class MoMoLibrary
    {
        /// <summary>
        /// Tạo chữ ký HMAC SHA256 cho MoMo
        /// </summary>
        public static string SignSHA256(string message, string key)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Tạo URL thanh toán MoMo
        /// </summary>
        public static async Task<string> CreatePaymentUrl(
            string endpoint,
            string partnerCode,
            string accessKey,
            string secretKey,
            string orderId,
            decimal amount,
            string orderInfo,
            string returnUrl,
            string ipnUrl,
            string requestType = "captureWallet",
            string extraData = "")
        {
            // Convert amount to long (MoMo requires integer)
            long amountLong = (long)amount;
            
            // Ensure extraData is empty string, not null
            if (string.IsNullOrEmpty(extraData))
            {
                extraData = "";
            }
            
            // Build raw signature exactly as MoMo documentation specifies
            // Order is critical: accessKey, amount, extraData, ipnUrl, orderId, orderInfo, partnerCode, redirectUrl, requestId, requestType
            string rawSignature = "accessKey=" + accessKey + 
                                  "&amount=" + amountLong + 
                                  "&extraData=" + extraData + 
                                  "&ipnUrl=" + ipnUrl + 
                                  "&orderId=" + orderId + 
                                  "&orderInfo=" + orderInfo + 
                                  "&partnerCode=" + partnerCode + 
                                  "&redirectUrl=" + returnUrl + 
                                  "&requestId=" + orderId + 
                                  "&requestType=" + requestType;
            
            // Debug log
            Console.WriteLine($"[MOMO DEBUG] Raw Signature String: {rawSignature}");
            Console.WriteLine($"[MOMO DEBUG] Secret Key: {secretKey.Substring(0, 5)}...");
            
            string signature = SignSHA256(rawSignature, secretKey);
            
            Console.WriteLine($"[MOMO DEBUG] Signature: {signature}");

            // Build request object
            var requestData = new
            {
                partnerCode = partnerCode,
                partnerName = "EV Rental",
                storeId = "EV_Rental_Store",
                requestId = orderId,
                amount = amountLong.ToString(),
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = ipnUrl,
                lang = "vi",
                extraData = extraData,
                requestType = requestType,
                signature = signature
            };
            
            Console.WriteLine($"[MOMO DEBUG] Request JSON: {JsonSerializer.Serialize(requestData, new JsonSerializerOptions { WriteIndented = true })}");

            // Send request to MoMo
            using (var client = new HttpClient())
            {
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"[MOMO DEBUG] Response: {responseContent}");
                
                // Parse response
                var momoResponse = JsonSerializer.Deserialize<MoMoPaymentResponse>(responseContent);
                
                if (momoResponse != null && momoResponse.ResultCode == 0)
                {
                    return momoResponse.PayUrl ?? string.Empty;
                }
                else
                {
                    throw new Exception($"MoMo Error: {momoResponse?.Message ?? "Unknown error"}");
                }
            }
        }

        /// <summary>
        /// Xác thực chữ ký từ MoMo callback
        /// </summary>
        public static bool ValidateSignature(
            string partnerCode,
            string orderId,
            string requestId,
            decimal amount,
            string orderInfo,
            string orderType,
            string transId,
            string resultCode,
            string message,
            string payType,
            string responseTime,
            string extraData,
            string signature,
            string secretKey)
        {
            string rawSignature = $"accessKey={string.Empty}&amount={amount}&extraData={extraData}&message={message}&orderId={orderId}&orderInfo={orderInfo}&orderType={orderType}&partnerCode={partnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";
            
            // Note: For return URL, MoMo doesn't include accessKey in signature
            // Build signature without accessKey for validation
            rawSignature = $"amount={amount}&extraData={extraData}&message={message}&orderId={orderId}&orderInfo={orderInfo}&orderType={orderType}&partnerCode={partnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";
            
            string expectedSignature = SignSHA256(rawSignature, secretKey);
            
            return signature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);
        }
    }

    // Response model for MoMo payment creation
    public class MoMoPaymentResponse
    {
        [JsonPropertyName("partnerCode")]
        public string? PartnerCode { get; set; }

        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }

        [JsonPropertyName("requestId")]
        public string? RequestId { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("responseTime")]
        public long ResponseTime { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("resultCode")]
        public int ResultCode { get; set; }

        [JsonPropertyName("payUrl")]
        public string? PayUrl { get; set; }

        [JsonPropertyName("deeplink")]
        public string? Deeplink { get; set; }

        [JsonPropertyName("qrCodeUrl")]
        public string? QrCodeUrl { get; set; }
    }
}
