using BusinessLayer.DTOs;
using System.Text.Json;

namespace EV_Rental.Helpers
{
    public static class SessionHelper
    {
        private const string USER_SESSION_KEY = "UserSession";
        private const string REGISTER_OTP_SESSION_KEY = "RegisterOtpSession";

        /// <summary>
        /// Lưu thông tin user vào session
        /// </summary>
        public static void SetUserSession(ISession session, AuthResponseDto user)
        {
            var userJson = JsonSerializer.Serialize(user);
            session.SetString(USER_SESSION_KEY, userJson);
        }

        /// <summary>
        /// Lấy thông tin user từ session
        /// </summary>
        public static AuthResponseDto? GetUserSession(ISession session)
        {
            var userJson = session.GetString(USER_SESSION_KEY);
            if (string.IsNullOrEmpty(userJson))
                return null;

            return JsonSerializer.Deserialize<AuthResponseDto>(userJson);
        }

        /// <summary>
        /// Kiểm tra user đã đăng nhập chưa
        /// </summary>
        public static bool IsLoggedIn(ISession session)
        {
            return !string.IsNullOrEmpty(session.GetString(USER_SESSION_KEY));
        }

        /// <summary>
        /// Xóa session (logout)
        /// </summary>
        public static void ClearSession(ISession session)
        {
            session.Clear();
        }

        public class RegisterOtpSession
        {
            public BusinessLayer.DTOs.RegisterRequestDto RegisterRequest { get; set; }
            public string OtpCode { get; set; }
            public DateTime ExpireAtUtc { get; set; }
            public int ResendCount { get; set; }
        }

        public static void SetRegisterOtpSession(ISession session, RegisterOtpSession data)
        {
            var json = JsonSerializer.Serialize(data);
            session.SetString(REGISTER_OTP_SESSION_KEY, json);
        }

        public static RegisterOtpSession? GetRegisterOtpSession(ISession session)
        {
            var json = session.GetString(REGISTER_OTP_SESSION_KEY);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<RegisterOtpSession>(json);
        }

        public static void ClearRegisterOtp(ISession session)
        {
            session.Remove(REGISTER_OTP_SESSION_KEY);
        }

        /// <summary>
        /// Kiểm tra user có role Admin không
        /// </summary>
        public static bool IsAdmin(ISession session)
        {
            var user = GetUserSession(session);
            return user?.Role == DataAccessLayer.Enums.AccountRole.Admin;
        }

        /// <summary>
        /// Kiểm tra user có role Staff không
        /// </summary>
        public static bool IsStaff(ISession session)
        {
            var user = GetUserSession(session);
            return user?.Role == DataAccessLayer.Enums.AccountRole.Staff;
        }

        /// <summary>
        /// Kiểm tra user có role Renter không
        /// </summary>
        public static bool IsRenter(ISession session)
        {
            var user = GetUserSession(session);
            return user?.Role == DataAccessLayer.Enums.AccountRole.Renter;
        }

        /// <summary>
        /// Lấy role của user hiện tại
        /// </summary>
        public static string? GetUserRole(ISession session)
        {
            var user = GetUserSession(session);
            return user?.Role.ToString();
        }
    }
}
