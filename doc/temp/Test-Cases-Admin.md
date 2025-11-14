# Test Cases - Tính năng Admin EV Rental

## TC-01: Quản lý Báo cáo Sự cố

### TC-01.1: Xem danh sách báo cáo sự cố
**Mục đích:** Kiểm tra hiển thị danh sách InspectionProblem

**Điều kiện tiên quyết:**
- Đăng nhập với tài khoản Admin
- Database có ít nhất 1 InspectionProblem

**Các bước thực hiện:**
1. Đăng nhập Admin
2. Click menu "Báo cáo Sự cố"
3. Quan sát trang hiển thị

**Kết quả mong đợi:**
- ✅ Hiển thị URL: `/Admin/Problems/Index`
- ✅ Hiển thị bảng với các cột: ID, Đơn thuê, Khách hàng, Xe, Loại sự cố, Mô tả, Phí phạt, Ngày tạo, Thao tác
- ✅ Hiển thị thống kê: Tổng số sự cố, Tổng phạt
- ✅ Có dropdown filter loại sự cố
- ✅ Có ô tìm kiếm

---

### TC-01.2: Lọc theo loại sự cố
**Các bước:**
1. Vào trang Báo cáo Sự cố
2. Chọn dropdown "Loại sự cố" = "Hư hỏng"
3. Click "Lọc"

**Kết quả:**
- ✅ Chỉ hiển thị các báo cáo có IncidentType = "damage"
- ✅ Badge màu đỏ "Hư hỏng"

---

### TC-01.3: Tìm kiếm báo cáo
**Các bước:**
1. Nhập tên khách hàng vào ô tìm kiếm
2. Click "Lọc"

**Kết quả:**
- ✅ Hiển thị các báo cáo có RenterName khớp
- ✅ Hoặc VehicleName khớp
- ✅ Hoặc Description khớp

---

### TC-01.4: Xem chi tiết báo cáo
**Các bước:**
1. Click button "Xem chi tiết" (icon eye)
2. Quan sát modal popup

**Kết quả:**
- ✅ Modal hiển thị với title "Chi tiết báo cáo sự cố"
- ✅ Hiển thị đầy đủ: ID, Đơn thuê (có link), Khách hàng, Xe, Loại sự cố, Phí phạt, Ngày tạo, Mô tả chi tiết

---

### TC-01.5: Điều xe đi bảo trì
**Các bước:**
1. Click button "Điều xe bảo trì" (icon wrench)
2. Confirm dialog
3. Kiểm tra status xe

**Kết quả:**
- ✅ Hiển thị confirm "Bạn có chắc chắn muốn điều xe này đi bảo trì?"
- ✅ Sau khi confirm, hiển thị "Đã chuyển xe sang trạng thái bảo trì"
- ✅ Check database: Vehicle.Status = Maintenance
- ✅ Vào trang Quản lý Xe → Xe có badge "Bảo trì"

---

### TC-01.6: Khóa tài khoản khách vi phạm
**Các bước:**
1. Click button "Khóa tài khoản" (icon ban)
2. Confirm dialog
3. Kiểm tra account status

**Kết quả:**
- ✅ Hiển thị confirm "Bạn có chắc chắn muốn khóa tài khoản khách hàng này?"
- ✅ Sau khi confirm: "Đã khóa tài khoản khách hàng"
- ✅ Check database: Account.IsActive = false
- ✅ Khách hàng không thể đăng nhập

---

## TC-02: Thống Kê Nâng Cao

### TC-02.1: Xem thống kê tổng quan
**Các bước:**
1. Click menu "Thống Kê Nâng Cao"
2. Quan sát các sections

**Kết quả:**
- ✅ Hiển thị URL: `/Admin/Statistics`
- ✅ Section 1: Doanh thu theo trạm (table + bar chart)
- ✅ Section 2: Tần suất thuê theo ngày (line chart)
- ✅ Section 3: Thống kê theo loại xe (table + doughnut chart)
- ✅ Section 4: Top 10 xe được thuê nhiều nhất (table)

---

### TC-02.2: Filter theo khoảng thời gian
**Các bước:**
1. Chọn "Từ ngày" = 01/11/2025
2. Chọn "Đến ngày" = 14/11/2025
3. Click "Lọc"

**Kết quả:**
- ✅ Doanh thu theo trạm chỉ tính trong khoảng đó
- ✅ Daily frequency chart chỉ hiển thị từ 01/11 → 14/11
- ✅ URL có params: `?startDate=2025-11-01&endDate=2025-11-14`

---

### TC-02.3: Kiểm tra Bar Chart - Doanh thu theo trạm
**Kết quả:**
- ✅ Chart hiển thị đúng số trạm
- ✅ Bar height tương ứng với revenue
- ✅ Hover hiển thị tooltip với giá trị chính xác (triệu VNĐ)
- ✅ Chart responsive

---

### TC-02.4: Kiểm tra Line Chart - Tần suất theo ngày
**Kết quả:**
- ✅ X-axis: Các ngày trong khoảng filter (dd/MM)
- ✅ Y-axis: Số lượt thuê
- ✅ Line màu xanh, smooth curve
- ✅ Hover hiển thị số chính xác

---

### TC-02.5: Kiểm tra Doughnut Chart - Theo loại xe
**Kết quả:**
- ✅ Mỗi loại xe có 1 màu riêng
- ✅ Tỷ lệ phần trăm tương ứng số lượt thuê
- ✅ Legend hiển thị tên loại xe
- ✅ Hover hiển thị số lượt

---

### TC-02.6: Top 10 xe - Ranking icons
**Kết quả:**
- ✅ Top 1: Badge vàng với crown icon
- ✅ Top 2: Badge xám với medal icon
- ✅ Top 3: Badge đỏ với award icon
- ✅ Top 4-10: Badge trắng với số
- ✅ Progress bar completion rate hiển thị đúng %

---

## TC-03: Phân phối Xe hàng loạt

### TC-03.1: Xem trang phân phối
**Các bước:**
1. Vào "Quản lý Xe"
2. Click button "Phân phối hàng loạt"

**Kết quả:**
- ✅ Hiển thị URL: `/Admin/Vehicle/BulkAssign`
- ✅ Bên trái: Bảng xe với checkbox
- ✅ Bên phải: Panel "Phân phối" (sticky)
- ✅ Hiển thị "Đã chọn: 0 xe"

---

### TC-03.2: Filter theo trạm hiện tại
**Các bước:**
1. Dropdown "Trạm hiện tại" chọn một trạm
2. Click "Lọc"

**Kết quả:**
- ✅ Chỉ hiển thị xe thuộc trạm đó
- ✅ Badge trạm khớp với filter
- ✅ URL có param: `?currentStation=X`

---

### TC-03.3: Filter theo trạng thái
**Các bước:**
1. Dropdown "Trạng thái" chọn "Sẵn sàng"
2. Click "Lọc"

**Kết quả:**
- ✅ Chỉ hiển thị xe Available
- ✅ Badge màu xanh "Sẵn sàng"
- ✅ Không có xe Rented (đã exclude)

---

### TC-03.4: Select All checkbox
**Các bước:**
1. Click checkbox "Select All" ở header
2. Quan sát

**Kết quả:**
- ✅ Tất cả checkbox xe được tick
- ✅ "Đã chọn: X xe" cập nhật đúng
- ✅ Button "Xác nhận Phân phối" enabled
- ✅ Uncheck → Tất cả bỏ tick

---

### TC-03.5: Select từng xe
**Các bước:**
1. Tick checkbox 3 xe bất kỳ
2. Quan sát số đếm

**Kết quả:**
- ✅ "Đã chọn: 3 xe"
- ✅ Button enabled
- ✅ Hidden input có value = "id1,id2,id3"

---

### TC-03.6: Phân phối xe
**Điều kiện:** Đã chọn 3 xe

**Các bước:**
1. Dropdown "Trạm đích" chọn trạm X
2. (Tùy chọn) Dropdown "Cập nhật trạng thái" chọn "Sạc pin"
3. Click "Xác nhận Phân phối"
4. Confirm dialog

**Kết quả:**
- ✅ Dialog: "Bạn có chắc chắn muốn phân phối 3 xe đến Trạm X?"
- ✅ Sau confirm: Redirect về `/Admin/Vehicle/Index`
- ✅ TempData success: "Đã phân phối 3 xe đến trạm X"
- ✅ Check database: 3 xe có StationId = X
- ✅ Nếu chọn status: 3 xe có Status = Charging

---

### TC-03.7: Validation - Không chọn xe
**Các bước:**
1. Không tick xe nào
2. Click "Xác nhận Phân phối"

**Kết quả:**
- ✅ Button disabled, không submit được

---

### TC-03.8: Validation - Không chọn trạm đích
**Các bước:**
1. Tick 2 xe
2. Không chọn trạm đích
3. Click "Xác nhận Phân phối"

**Kết quả:**
- ✅ HTML5 validation: "Please select an item in the list"

---

## TC-04: Integration Tests

### TC-04.1: Workflow đầy đủ - Xử lý sự cố
**Scenario:** Staff báo cáo xe hư → Admin xử lý

**Các bước:**
1. (Staff) Tạo InspectionProblem cho rental X
2. (Admin) Vào Báo cáo Sự cố → Thấy báo cáo mới
3. Click "Điều xe bảo trì"
4. Vào Quản lý Xe → Kiểm tra xe có status Maintenance
5. Vào Phân phối hàng loạt → Xe Maintenance vẫn hiển thị
6. Phân phối xe về trạm bảo trì, đổi status thành "Bảo trì"

**Kết quả:**
- ✅ Workflow hoàn chỉnh không lỗi
- ✅ Status và StationId cập nhật đúng

---

### TC-04.2: Workflow - Ban user
**Các bước:**
1. Có InspectionProblem với RenterId = Y
2. Admin vào Báo cáo Sự cố
3. Click "Khóa tài khoản"
4. Logout admin
5. Thử login bằng account Y

**Kết quả:**
- ✅ Account Y không login được
- ✅ Hiển thị message "Tài khoản đã bị khóa"

---

### TC-04.3: Thống kê realtime
**Các bước:**
1. Vào Statistics, note số liệu hiện tại
2. Tạo rental mới (completed)
3. Refresh Statistics

**Kết quả:**
- ✅ Daily frequency tăng +1
- ✅ Revenue tăng đúng giá trị
- ✅ Top vehicles cập nhật

---

## Test Data Setup

### Chuẩn bị test data
```sql
-- 1. Create test stations
INSERT INTO Stations (Name, Address, State) VALUES 
('Trạm Quận 1', '123 Nguyễn Huệ', 'Hồ Chí Minh'),
('Trạm Quận 7', '456 Nguyễn Văn Linh', 'Hồ Chí Minh');

-- 2. Create test vehicles
INSERT INTO Vehicles (StationId, Name, PlateNumber, Status, VehicleType) VALUES
(1, 'VinFast VF7', '51A-12345', 'Available', 'car'),
(1, 'VinFast VF8', '51A-67890', 'Available', 'car'),
(2, 'VinFast VF5', '51B-11111', 'Maintenance', 'car');

-- 3. Create test inspection problem
INSERT INTO InspectionProblems (RentalId, IncidentType, Description, PenaltyAmount) VALUES
(1, 'damage', 'Xe bị trầy xước cửa', 500000);
```

---

## Browser Compatibility

Test trên các trình duyệt:
- ✅ Chrome (latest)
- ✅ Firefox (latest)
- ✅ Edge (latest)
- ⚠️ Safari (check chart.js compatibility)

---

## Performance Tests

### Load Test
- ✅ 100 vehicles → BulkAssign page load < 2s
- ✅ 1000 rentals → Statistics charts render < 3s
- ✅ 50 problems → Problems page load < 1s

---

**Tổng số test cases:** 26  
**Ước tính thời gian test:** 2-3 giờ  
**Người tạo:** GitHub Copilot  
**Ngày:** 14/11/2025
