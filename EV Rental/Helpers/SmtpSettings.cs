namespace EV_Rental.Helpers
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}


