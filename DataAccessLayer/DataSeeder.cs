using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public static class DataSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Admin Account
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    FullName = "Administrator",
                    Email = "admin@evrental.com",
                    Phone = "0123456789",
                    PasswordHash = "Admin@123", // Trong thực tế nên hash password
                    Role = AccountRole.Admin,
                    IsActive = true,
                    CreateDate = new DateTime(2025, 10, 24),
                    UpdateDate = new DateTime(2025, 10, 24),
                    IsDeleted = false
                }
            );

            // Có thể thêm seed data khác ở đây
            // Ví dụ: Station, Vehicle, etc.
        }
    }
}
