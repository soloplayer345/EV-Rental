# Tính năng Quản lý Users - EV Rental Admin

## Tổng quan
Tính năng quản lý users cho phép Admin quản lý tất cả người dùng trong hệ thống EV Rental, bao gồm Renter, Staff và Admin.

## Ngày tạo
07/11/2025

## Files đã tạo

### 1. Frontend (Razor Page)
- **File**: `EV Rental\Pages\Admin\Users.cshtml`
- **Mô tả**: Giao diện quản lý users với các tính năng:
  - Hiển thị danh sách users dạng bảng
  - Thống kê tổng quan (Tổng users, Users hoạt động, Users chờ duyệt, Users mới tháng này)
  - Tìm kiếm và lọc theo tên, email, số điện thoại, vai trò, trạng thái
  - Thêm user mới
  - Chỉnh sửa thông tin user
  - Xem chi tiết user
  - Kích hoạt/Vô hiệu hóa user
  - Xóa user (soft delete)

### 2. Backend (C# Code-behind)
- **File**: `EV Rental\Pages\Admin\Users.cshtml.cs`
- **Mô tả**: Logic xử lý cho trang quản lý users
- **Class**: `UsersModel : PageModel`

## Chức năng chính

### 1. Hiển thị danh sách Users
```csharp
public async Task<IActionResult> OnGetAsync()
```
- Kiểm tra quyền Admin
- Lấy tất cả users từ database
- Áp dụng bộ lọc (tìm kiếm, vai trò, trạng thái)
- Tính toán thống kê

### 2. Thêm User mới
```csharp
public async Task<IActionResult> OnPostAddUserAsync(...)
```
- Validate dữ liệu đầu vào
- Kiểm tra email và phone đã tồn tại chưa
- Hash mật khẩu bằng BCrypt
- Tạo user mới trong database

**Tham số**:
- FullName (string): Họ tên
- Email (string): Email
- Phone (string): Số điện thoại
- Role (int): Vai trò (0=Renter, 1=Staff, 2=Admin)
- Password (string): Mật khẩu
- ConfirmPassword (string): Xác nhận mật khẩu
- IsActive (bool): Trạng thái kích hoạt

### 3. Xem chi tiết User
```csharp
public async Task<IActionResult> OnGetUserDetailsAsync(int userId)
```
- Trả về JSON với thông tin chi tiết của user
- Bao gồm số lượt thuê xe

### 4. Chỉnh sửa User
```csharp
public async Task<IActionResult> OnPostEditUserAsync(...)
```
- Cập nhật thông tin user
- Kiểm tra email và phone nếu có thay đổi

**Tham số**:
- UserId (int): ID của user
- FullName (string): Họ tên mới
- Email (string): Email mới
- Phone (string): Số điện thoại mới
- Role (int): Vai trò mới
- IsActive (bool): Trạng thái mới

### 5. Kích hoạt/Vô hiệu hóa User
```csharp
public async Task<IActionResult> OnPostToggleStatusAsync(int userId, bool isActive)
```
- Thay đổi trạng thái IsActive của user
- Trả về JSON response

### 6. Xóa User
```csharp
public async Task<IActionResult> OnPostDeleteUserAsync(int userId)
```
- Soft delete: đặt IsDeleted = true
- Không xóa vĩnh viễn khỏi database
- Trả về JSON response

## Thống kê hiển thị

1. **Tổng Users**: Tổng số users trong hệ thống
2. **Users Hoạt động**: Số users có IsActive = true
3. **Users Chờ duyệt**: Số users có IsActive = false
4. **Users mới tháng này**: Số users được tạo trong tháng hiện tại

## Bộ lọc

### Search
- Tìm kiếm theo: Họ tên, Email, Số điện thoại
- Case-insensitive

### Role Filter
- Tất cả vai trò
- Renter (0)
- Staff (1)
- Admin (2)

### Status Filter
- Tất cả
- Hoạt động (true)
- Chờ duyệt (false)

## Bảng Users

### Cột hiển thị:
1. ID
2. Họ tên (với avatar)
3. Email
4. Số điện thoại
5. Vai trò (badge màu)
6. Trạng thái (badge màu)
7. Ngày tạo
8. Thao tác (buttons)

### Thao tác trên mỗi user:
- 👁️ Xem chi tiết
- ✏️ Chỉnh sửa
- ✅ Kích hoạt / ❌ Vô hiệu hóa
- 🗑️ Xóa

## Modals

### 1. Add User Modal
- Form nhập thông tin user mới
- Validation trên client và server
- Hash password tự động

### 2. Edit User Modal
- Load dữ liệu user qua AJAX
- Form chỉnh sửa thông tin
- Không cho phép đổi password (cần làm riêng)

### 3. View User Modal
- Hiển thị thông tin chi tiết
- Avatar lớn với initial
- Thông tin đầy đủ về user

## Dependencies

### Packages đã cài:
```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

### Sử dụng:
- `DataAccessLayer.Interfaces.IUnitOfWork`
- `DataAccessLayer.Entities.Account`
- `DataAccessLayer.Enums.AccountRole`
- `EV_Rental.Helpers.SessionHelper`
- `BCrypt.Net.BCrypt`

## Security

### Authentication
- Kiểm tra đăng nhập qua `SessionHelper.IsLoggedIn()`
- Chỉ cho phép Admin truy cập

### Authorization
- Redirect về trang chủ nếu không phải Admin

### CSRF Protection
- Sử dụng `@Html.AntiForgeryToken()` trong forms
- Validate token trong POST requests

### Password Security
- Hash password bằng BCrypt
- Không hiển thị password
- Không cho phép thay đổi password qua form edit

## UI/UX Features

### Icons (Font Awesome)
- fa-users: Users
- fa-user-check: Active users
- fa-user-clock: Pending users
- fa-user-plus: New users / Add user
- fa-eye: View details
- fa-edit: Edit
- fa-ban: Deactivate
- fa-check: Activate
- fa-trash: Delete

### Badges
- **Renter**: Badge info (xanh dương)
- **Staff**: Badge warning (vàng)
- **Admin**: Badge danger (đỏ)
- **Hoạt động**: Badge success (xanh lá)
- **Chờ duyệt**: Badge secondary (xám)

### Responsive
- Mobile-friendly
- Responsive table
- Modal dialogs

## JavaScript Functions

### Main Functions
```javascript
viewUser(userId)           // Xem chi tiết user
editUser(userId)           // Mở form chỉnh sửa
toggleUserStatus(userId, newStatus)  // Đổi trạng thái
deleteUser(userId)         // Xóa user
exportToExcel()            // Xuất Excel (TODO)
```

## Navigation

### Access URL
```
/Admin/Users
```

### Sidebar Menu
- Menu item "Quản lý Users" với icon fa-users
- Active highlighting tự động

## Future Enhancements

1. ✅ Export to Excel
2. ✅ Import users from CSV/Excel
3. ✅ Bulk actions (activate/deactivate multiple users)
4. ✅ Advanced filters (date range, rental count)
5. ✅ Pagination (hiện tại hiển thị tất cả)
6. ✅ Reset password functionality
7. ✅ Send email to users
8. ✅ User activity log
9. ✅ Role management (thêm roles mới)
10. ✅ Permission management

## Testing

### Test Cases
1. ✅ Admin có thể xem danh sách users
2. ✅ Admin có thể thêm user mới
3. ✅ Admin có thể chỉnh sửa thông tin user
4. ✅ Admin có thể xem chi tiết user
5. ✅ Admin có thể kích hoạt/vô hiệu hóa user
6. ✅ Admin có thể xóa user
7. ✅ Validate email và phone không trùng
8. ✅ Validate password match
9. ✅ Non-admin không thể truy cập
10. ✅ Search và filter hoạt động đúng

## Notes

- Tất cả các thay đổi được ghi log với `CreateDate` và `UpdateDate`
- Soft delete: `IsDeleted = true`, không xóa vĩnh viễn
- Password được hash bằng BCrypt trước khi lưu
- AJAX được sử dụng cho các actions (toggle status, delete)
- Form submissions sử dụng standard POST với page handlers

## Author
Created on November 7, 2025
