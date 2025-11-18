using Microsoft.Data.SqlClient;

namespace EV_Rental.Helpers
{
    /// <summary>
    /// Helper class để quản lý database connection với cơ chế fallback tự động
    /// </summary>
    public static class DatabaseConnectionHelper
    {
        /// <summary>
        /// Kiểm tra connection string và fallback sang Local nếu server ngoài sập
        /// </summary>
        public static string GetConnectionStringWithFallback(IConfiguration configuration)
        {
            var defaultConnection = configuration.GetConnectionString("DefaultConnection");
            var localConnection = configuration.GetConnectionString("LocalSQLServer");

            try
            {
                // Test connection đến server ngoài
                Console.WriteLine("🔍 Đang kiểm tra kết nối đến server ngoài...");
                using (var connection = new SqlConnection(defaultConnection))
                {
                    connection.Open();
                    Console.WriteLine("✅ Kết nối thành công đến server ngoài!");
                    return defaultConnection!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Không thể kết nối đến server ngoài: {ex.Message}");
                Console.WriteLine("🔄 Đang chuyển sang sử dụng Local SQL Server...");
                
                try
                {
                    // Test connection đến local
                    using (var connection = new SqlConnection(localConnection))
                    {
                        connection.Open();
                        Console.WriteLine("✅ Kết nối thành công đến Local SQL Server!");
                        return localConnection!;
                    }
                }
                catch (Exception localEx)
                {
                    Console.WriteLine($"❌ Không thể kết nối đến Local SQL Server: {localEx.Message}");
                    Console.WriteLine("💡 Vui lòng kiểm tra:");
                    Console.WriteLine("   1. SQL Server đã được bật chưa");
                    Console.WriteLine("   2. Connection string trong appsettings.json");
                    Console.WriteLine("   3. SQL Server Authentication mode");
                    throw new InvalidOperationException("Không thể kết nối đến bất kỳ database nào!", localEx);
                }
            }
        }
    }
}
