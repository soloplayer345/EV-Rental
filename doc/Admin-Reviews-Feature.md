# Trang Báo Cáo Đánh Giá - Admin Dashboard

## Tổng Quan
Trang báo cáo đánh giá (Reviews Report) cho phép admin xem và quản lý tất cả các đánh giá từ khách hàng về dịch vụ cho thuê xe điện.

## Các Tính Năng Đã Triển Khai

### 1. **Repository Layer**
**File:** `DataAccessLayer/Repositories/RatingReviewRepo.cs`

Các phương thức:
- `GetAllWithDetailsAsync()` - Lấy tất cả đánh giá kèm thông tin chi tiết (khách hàng, xe)
- `GetByIdWithDetailsAsync(int id)` - Lấy một đánh giá theo ID
- `GetByVehicleIdAsync(int vehicleId)` - Lấy đánh giá theo xe
- `GetAverageRatingForVehicleAsync(int vehicleId)` - Tính điểm trung bình cho xe
- `GetRatingDistributionAsync()` - Phân bố đánh giá từ 1-5 sao

### 2. **Service Layer**
**File:** `BusinessLayer/Services/ReviewService.cs`

Các phương thức:
- `GetAllReviewsAsync()` - Lấy tất cả đánh giá
- `GetReviewStatisticsAsync()` - Lấy thống kê tổng quan
- `GetReviewsByVehicleAsync(int vehicleId)` - Lấy đánh giá theo xe

### 3. **DTOs (Data Transfer Objects)**
**File:** `BusinessLayer/DTOs/RatingReviewDto.cs`

**RatingReviewDto:**
- Id, RentalId, Rating, Comment
- Thông tin khách hàng: RenterName, RenterEmail
- Thông tin xe: VehicleId, VehicleName, VehiclePlateNumber, VehicleImageUrl
- CreatedAt: Ngày tạo đánh giá

**ReviewStatisticsDto:**
- TotalReviews: Tổng số đánh giá
- AverageRating: Điểm trung bình
- RatingDistribution: Phân bố theo từng sao (1-5)
- Count và Percentage cho mỗi mức sao

### 4. **Admin Page**
**Files:** 
- `EV Rental/Pages/Admin/Reviews.cshtml` (View)
- `EV Rental/Pages/Admin/Reviews.cshtml.cs` (Code-behind)

#### Các Thành Phần Giao Diện:

##### a) **Statistics Cards (Thẻ Thống Kê)**
- Tổng đánh giá
- Điểm trung bình
- Số lượng 5 sao
- Số lượng 1-2 sao (đánh giá kém)

##### b) **Rating Distribution Chart (Biểu Đồ Phân Bố)**
- Hiển thị phân bố từ 5 sao đến 1 sao
- Progress bar với màu sắc khác nhau
- Hiển thị số lượng và phần trăm

##### c) **Filter & Search (Bộ Lọc và Tìm Kiếm)**
- Tìm kiếm theo: Tên xe, biển số, tên khách hàng, email, nhận xét
- Lọc theo đánh giá: 1-5 sao
- Sắp xếp theo:
  - Mới nhất
  - Cũ nhất
  - Đánh giá cao nhất
  - Đánh giá thấp nhất

##### d) **Reviews Table (Bảng Danh Sách)**
Hiển thị:
- Hình ảnh và thông tin xe
- Thông tin khách hàng
- Đánh giá (badge màu theo mức sao)
- Nhận xét (truncate nếu quá dài)
- Ngày đánh giá
- Nút xem chi tiết

##### e) **Review Detail Modal (Popup Chi Tiết)**
Hiển thị đầy đủ:
- Hình ảnh xe lớn
- Tên xe và biển số
- Thông tin khách hàng
- Đánh giá chi tiết
- Nhận xét đầy đủ
- Ngày đánh giá

### 5. **URL và Navigation**
- URL: `/Admin/Reviews`
- Menu Admin: "Đánh giá" với icon sao
- Đã được tích hợp vào `_AdminLayout.cshtml`

## Cấu Trúc Database

**Table: RatingReview**
```sql
- Id (int, Primary Key)
- RentalId (int, Foreign Key to RentalRecord)
- Rating (int, 1-5)
- Comment (string, nullable)
- CreateDate (DateTime)
- UpdateDate (DateTime, nullable)
- IsDeleted (bool, default: false)
```

**Relationships:**
- RatingReview → RentalRecord (One-to-One)
- RentalRecord → Account (Renter) (Many-to-One)
- RentalRecord → Vehicle (Many-to-One)

## Cách Sử Dụng

### 1. Truy Cập Trang
1. Đăng nhập với tài khoản Admin
2. Click vào menu "Đánh giá" trên sidebar
3. Hoặc truy cập trực tiếp: `http://localhost:5126/Admin/Reviews`

### 2. Xem Thống Kê
- Dashboard hiển thị tổng quan ngay đầu trang
- Biểu đồ phân bố giúp đánh giá chất lượng dịch vụ tổng thể

### 3. Tìm Kiếm và Lọc
- Nhập từ khóa vào ô tìm kiếm
- Chọn mức đánh giá để lọc
- Chọn cách sắp xếp phù hợp
- Click "Lọc" để áp dụng

### 4. Xem Chi Tiết
- Click nút "Chi tiết" trên mỗi dòng
- Modal popup hiển thị thông tin đầy đủ

## Màu Sắc Badge Theo Rating
- 5 sao: **Xanh lá** (bg-success)
- 4 sao: **Xanh dương** (bg-info)
- 3 sao: **Vàng** (bg-warning)
- 2 sao: **Cam** (bg-orange)
- 1 sao: **Đỏ** (bg-danger)

## Dependencies Đã Đăng Ký

Trong `Program.cs`:
```csharp
builder.Services.AddScoped<IRatingReviewRepo, RatingReviewRepo>();
builder.Services.AddScoped<ReviewService>();
```

Trong `UnitOfWork.cs`:
```csharp
private IRatingReviewRepo? _ratingReviewRepo;
// Tự động khởi tạo RatingReviewRepo khi GetRepository<RatingReview>()
```

## Responsive Design
- Mobile-friendly
- Bootstrap 5 components
- Responsive tables
- Modal dialogs

## Security
- Chỉ Admin mới truy cập được
- Protected by authentication middleware
- Role-based access control

## Tương Lai - Các Tính Năng Có Thể Mở Rộng

1. **Export Reports**
   - Xuất báo cáo ra Excel/PDF
   - Báo cáo theo khoảng thời gian

2. **Response to Reviews**
   - Admin có thể trả lời đánh giá
   - Đánh dấu đã xử lý

3. **Email Notifications**
   - Thông báo khi có đánh giá mới
   - Alert khi có đánh giá kém

4. **Analytics Dashboard**
   - Trend theo thời gian
   - So sánh giữa các xe
   - Sentiment analysis

5. **Moderation Tools**
   - Ẩn/hiện đánh giá không phù hợp
   - Đánh dấu spam
   - Report abuse

## Troubleshooting

### Không thấy dữ liệu
- Kiểm tra database có RatingReview chưa
- Đảm bảo RentalRecord đã hoàn thành
- Kiểm tra kết nối database

### Lỗi khi build
- Chạy `dotnet restore`
- Kiểm tra các dependencies
- Xem các cảnh báo compile

### Không truy cập được trang
- Kiểm tra đã đăng nhập với role Admin
- Kiểm tra middleware RoleBasedRedirect
- Xem logs trong console

## Testing

Để test trang này:
1. Tạo một số RentalRecord với status "completed"
2. Thêm RatingReview cho các rental đó
3. Đăng nhập Admin và truy cập `/Admin/Reviews`

## Contact & Support
- GitHub Issues: [EV-Rental](https://github.com/soloplayer345/EV-Rental)
- Branch: Management-&-Reporting
