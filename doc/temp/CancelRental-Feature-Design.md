# THIẾT KẾ LUỒNG HỦY ĐƠN THUÊ XE CÓ CHÍNH SÁCH

**Ngày tạo:** 13/11/2025  
**Dự án:** EV Rental System  
**Tính năng:** Hủy đơn thuê xe với chính sách hoàn tiền

---

## 📋 MỤC LỤC

1. [Phân tích hiện trạng](#1-phân-tích-hiện-trạng)
2. [Thiết kế luồng mới](#2-thiết-kế-luồng-mới)
3. [Cấu trúc file cần tạo/chỉnh sửa](#3-cấu-trúc-file-cần-tạochỉnh-sửa)
4. [Luồng dữ liệu chi tiết](#4-luồng-dữ-liệu-chi-tiết)
5. [Logic tính tiền hoàn lại](#5-logic-tính-tiền-hoàn-lại)
6. [Database - Payment record cho Refund](#6-database---payment-record-cho-refund)
7. [UI/UX Considerations](#7-uiux-considerations)
8. [Validation & Error Handling](#8-validation--error-handling)
9. [Testing Scenarios](#9-testing-scenarios)
10. [Tóm tắt files cần tạo/sửa](#10-tóm-tắt-files-cần-tạosửa)
11. [Bonus: Chuẩn bị cho luồng Refund sau này](#11-bonus-chuẩn-bị-cho-luồng-refund-sau-này)

---

## 1. PHÂN TÍCH HIỆN TRẠNG

### Hiện tại đã có:
- ✅ `CancelRentalAsync()` trong `RentalService.cs` - xử lý hủy đơn cơ bản
- ✅ `OnPostCancelRentalAsync()` trong `MyTrips.cshtml.cs` - xử lý request từ UI
- ✅ Form hủy đơn trực tiếp trong `MyTrips.cshtml`
- ✅ Entity `Payment` với status có `refunded`
- ✅ `RentalRecordStatus` enum có `Cancelled`

### Vấn đề cần giải quyết:
Luồng hiện tại hủy trực tiếp mà không có:
- ❌ Hiển thị chính sách hủy cho user xem
- ❌ Yêu cầu user đồng ý với chính sách
- ❌ Tính toán số tiền hoàn lại theo chính sách
- ❌ Xử lý phí phạt hủy đơn (nếu có)
- ❌ Tạo record refund để xử lý sau

---

## 2. THIẾT KẾ LUỒNG MỚI

### Bước 1: User Click "Hủy đơn"
- Không hủy ngay
- Chuyển đến trang/modal hiển thị **Chính sách hủy đơn**

### Bước 2: Hiển thị Chính sách
Nội dung cần có:

```
📋 CHÍNH SÁCH HỦY ĐƠN THUÊ XE

1. Điều kiện hủy đơn:
   - Chỉ được hủy đơn có trạng thái: Pending hoặc Confirmed
   - Không thể hủy đơn đang thuê (Active) hoặc đã hoàn thành

2. Hoàn tiền:
   • Hủy trước 24 giờ: Hoàn 100% tiền thuê (BasePrice)
   • Hủy từ 12-24 giờ trước: Hoàn 50% tiền thuê
   • Hủy dưới 12 giờ: Không hoàn tiền thuê
   
   ⚠️ LƯU Ý:
   - Phí giữ chỗ (ReservationFee): KHÔNG hoàn lại
   - Phí cọc (DepositFee): KHÔNG hoàn lại
   - Chỉ hoàn lại tiền thuê cơ bản (BasePrice)

3. Thời gian xử lý:
   - Tiền sẽ được hoàn về tài khoản trong 3-5 ngày làm việc
   - (Luồng refund sẽ xử lý riêng)

□ Tôi đã đọc và đồng ý với chính sách hủy đơn

[Đồng ý và Hủy đơn]  [Quay lại]
```

### Bước 3: User Đồng ý
- User check vào checkbox "Đồng ý"
- Click "Đồng ý và Hủy đơn"
- Gửi request đến server

### Bước 4: Backend xử lý
1. Validate lại điều kiện hủy
2. **Tính toán số tiền hoàn lại** theo chính sách
3. Cập nhật status RentalRecord → `Cancelled`
4. **Tạo record Payment với status = "pending_refund"**
5. Cập nhật Vehicle status → `Available`
6. Return về kết quả

### Bước 5: Hiển thị kết quả
```
✅ HỦY ĐƠN THÀNH CÔNG

Số tiền được hoàn lại: XXX,XXX VNĐ
(Dựa theo chính sách hủy đơn)

Tiền sẽ được chuyển về tài khoản trong 3-5 ngày làm việc.

[Về trang chuyến đi của tôi]
```

---

## 3. CẤU TRÚC FILE CẦN TẠO/CHỈNH SỬA

### A. Backend (Business Layer)

#### **File mới 1: `CancellationPolicyDto.cs`**
**Đường dẫn:** `BusinessLayer/DTOs/CancellationPolicyDto.cs`

**Mục đích:** DTO chứa thông tin chính sách hủy và số tiền hoàn lại

**Thuộc tính:**
```csharp
public class CancellationPolicyDto
{
    public int RentalId { get; set; }
    public bool CanCancel { get; set; }
    public decimal RefundAmount { get; set; }
    public decimal BasePrice { get; set; }
    public decimal ReservationFee { get; set; }
    public decimal DepositFee { get; set; }
    public string PolicyText { get; set; }
    public string RefundPercentage { get; set; } // "100%", "50%", "0%"
    public DateTime StartTime { get; set; }
    public TimeSpan TimeUntilStart { get; set; }
}
```

#### **Chỉnh sửa: `RentalService.cs`**
**Đường dẫn:** `BusinessLayer/Services/RentalService.cs`

**Thêm method mới:**

1. **`GetCancellationPolicyAsync(int rentalId, int renterId)`**
   - Lấy thông tin đơn thuê
   - Kiểm tra quyền truy cập
   - Tính toán số tiền hoàn lại theo thời gian còn lại
   - Trả về `CancellationPolicyDto`

2. **`CancelRentalWithPolicyAsync(int rentalId, int renterId, bool agreedToPolicy)`**
   - Validate: user phải đồng ý chính sách (`agreedToPolicy = true`)
   - Validate: đơn phải ở trạng thái Pending/Confirmed
   - Tính số tiền hoàn lại
   - Update RentalRecord.Status = Cancelled
   - Gọi PaymentService.CreateRefundRecordAsync()
   - Update Vehicle.Status = Available
   - Trả về kết quả

#### **Chỉnh sửa: `PaymentService.cs`**
**Đường dẫn:** `BusinessLayer/Services/PaymentService.cs`

**Thêm method:**

**`CreateRefundRecordAsync(int rentalId, decimal refundAmount)`**
- Tạo Payment record với:
  - Amount = -refundAmount (số âm)
  - Method = "refund"
  - Status = "pending_refund"
  - TransactionRef = $"REFUND-{rentalId}-{timestamp}"
- Để luồng refund sau này xử lý

---

### B. Presentation Layer

#### **File mới 1: `CancelRentalPolicy.cshtml`**
**Đường dẫn:** `EV Rental/Pages/Renter/CancelRentalPolicy.cshtml`

**Mục đích:** UI hiển thị chính sách hủy đơn

**Nội dung:**
- Hiển thị thông tin đơn thuê (xe, thời gian, giá)
- Hiển thị chi tiết chính sách hủy đơn
- Hiển thị số tiền sẽ được hoàn lại (tính theo thời gian)
- Checkbox "Đồng ý với chính sách"
- Button "Đồng ý và Hủy đơn" (disabled khi chưa check)
- Button "Quay lại"
- JavaScript enable/disable button theo checkbox

#### **File mới 2: `CancelRentalPolicy.cshtml.cs`**
**Đường dẫn:** `EV Rental/Pages/Renter/CancelRentalPolicy.cshtml.cs`

**Mục đích:** Logic xử lý hủy đơn có chính sách

**Methods:**
- **`OnGetAsync(int rentalId)`**
  - Validate user là Renter
  - Gọi `RentalService.GetCancellationPolicyAsync()`
  - Bind data vào page model
  
- **`OnPostAsync(int rentalId, bool agreedToPolicy)`**
  - Validate user và checkbox
  - Gọi `RentalService.CancelRentalWithPolicyAsync()`
  - Set TempData message
  - Redirect về MyTrips

#### **Chỉnh sửa: `MyTrips.cshtml`**
**Đường dẫn:** `EV Rental/Pages/Renter/MyTrips.cshtml`

**Thay đổi:**
- **Xóa:** Form post trực tiếp để hủy đơn
- **Thêm:** Link button đến trang CancelRentalPolicy

```html
<!-- Cũ -->
<form method="post" asp-page-handler="CancelRental">
    <input type="hidden" name="rentalId" value="@rental.Id" />
    <button type="submit">Hủy đơn</button>
</form>

<!-- Mới -->
<a href="/Renter/CancelRentalPolicy?rentalId=@rental.Id" 
   class="btn btn-danger">
    Hủy đơn
</a>
```

#### **Chỉnh sửa: `MyTrips.cshtml.cs`**
**Đường dẫn:** `EV Rental/Pages/Renter/MyTrips.cshtml.cs`

**Thay đổi:**
- **Xóa/Deprecated:** Method `OnPostCancelRentalAsync()` (không dùng trực tiếp nữa)
- Hoặc giữ lại nhưng thêm comment warning

---

## 4. LUỒNG DỮ LIỆU CHI TIẾT

```
MyTrips.cshtml
   ↓ (Click "Hủy đơn" - Link redirect)
   
CancelRentalPolicy.cshtml
   ↓ (Page Load)
   
CancelRentalPolicyModel.OnGetAsync(rentalId)
   ↓
   
RentalService.GetRentalByIdAsync(rentalId)
   ↓ (Validate quyền truy cập)
   
RentalService.GetCancellationPolicyAsync(rentalId)
   ↓ (Tính toán policy)
   ├─ Lấy thông tin rental
   ├─ Tính TimeUntilStart = StartTime - Now
   ├─ Áp dụng rule chính sách:
   │     • >= 24h → Hoàn 100% BasePrice
   │     • 12-24h → Hoàn 50% BasePrice
   │     • < 12h → Hoàn 0%
   └─ Return CancellationPolicyDto
   
   ↓ (Hiển thị chính sách + số tiền hoàn)
   
User đọc và check "Đồng ý"
   ↓ (Click "Đồng ý và Hủy đơn")
   
CancelRentalPolicyModel.OnPostAsync(rentalId, agreedToPolicy=true)
   ↓
   
RentalService.CancelRentalWithPolicyAsync(rentalId, renterId, true)
   ↓ (Validation & Processing)
   ├─ Validate user có quyền hủy
   ├─ Validate status (Pending/Confirmed only)
   ├─ Validate agreedToPolicy = true
   ├─ Validate chưa quá StartTime
   ├─ Tính số tiền hoàn lại (dùng lại logic của GetCancellationPolicy)
   ├─ Update RentalRecord.Status = Cancelled
   ├─ PaymentService.CreateRefundRecordAsync(rentalId, refundAmount)
   │     └─ Insert Payment(
   │           RentalId = rentalId,
   │           Amount = -refundAmount,  // Số âm
   │           Method = "refund",
   │           Status = "pending_refund",
   │           TransactionRef = "REFUND-xxx-xxx"
   │        )
   └─ VehicleService.UpdateVehicleAsync() → Status = Available
   
   ↓ (Commit transaction)
   
UnitOfWork.SaveChangesAsync()
   ↓
   
Redirect về MyTrips.cshtml
   └─ TempData["SuccessMessage"] = "Hủy đơn thành công..."
```

---

## 5. LOGIC TÍNH TIỀN HOÀN LẠI

### Pseudo-code trong `GetCancellationPolicyAsync()`

```csharp
public async Task<ServiceResultDto<CancellationPolicyDto>> GetCancellationPolicyAsync(
    int rentalId, int renterId)
{
    try
    {
        // 1. Lấy thông tin đơn thuê
        var rental = await GetRentalByIdAsync(rentalId);
        if (rental == null)
            return ServiceResultDto<CancellationPolicyDto>.FailureResult("Không tìm thấy đơn thuê.");
        
        // 2. Kiểm tra quyền truy cập
        if (rental.RenterId != renterId)
            return ServiceResultDto<CancellationPolicyDto>.FailureResult("Bạn không có quyền truy cập đơn này.");
        
        // 3. Kiểm tra trạng thái
        if (rental.Status != RentalRecordStatus.Pending && 
            rental.Status != RentalRecordStatus.Confirmed)
        {
            return ServiceResultDto<CancellationPolicyDto>.FailureResult(
                "Chỉ có thể hủy đơn ở trạng thái Chờ Thanh Toán hoặc Đã Xác Nhận.");
        }
        
        // 4. Tính thời gian còn lại đến StartTime
        var timeUntilStart = rental.StartTime.Value - DateTime.Now;
        
        // 5. Áp dụng chính sách hoàn tiền
        decimal refundAmount = 0;
        string policyText = "";
        string refundPercentage = "";
        bool canCancel = false;
        
        if (timeUntilStart.TotalHours < 0)
        {
            // Đã quá giờ StartTime → Không cho hủy
            policyText = "Không thể hủy đơn đã quá thời gian bắt đầu.";
            refundPercentage = "0%";
            canCancel = false;
        }
        else if (timeUntilStart.TotalHours >= 24)
        {
            // Hủy trước 24h → Hoàn 100%
            refundAmount = rental.BasePrice;
            policyText = "Hoàn 100% tiền thuê cơ bản vì hủy trước 24 giờ.";
            refundPercentage = "100%";
            canCancel = true;
        }
        else if (timeUntilStart.TotalHours >= 12)
        {
            // Hủy từ 12-24h → Hoàn 50%
            refundAmount = rental.BasePrice * 0.5m;
            policyText = "Hoàn 50% tiền thuê cơ bản (phí hủy 50%) vì hủy trong khoảng 12-24 giờ trước.";
            refundPercentage = "50%";
            canCancel = true;
        }
        else
        {
            // Hủy dưới 12h → Không hoàn
            refundAmount = 0;
            policyText = "Không hoàn tiền thuê vì hủy quá gần giờ nhận xe (dưới 12 giờ).";
            refundPercentage = "0%";
            canCancel = true;
        }
        
        // 6. Tạo DTO
        var policyDto = new CancellationPolicyDto
        {
            RentalId = rental.Id,
            CanCancel = canCancel,
            RefundAmount = refundAmount,
            BasePrice = rental.BasePrice,
            ReservationFee = rental.ReservationFee,  // Không hoàn
            DepositFee = rental.DepositFee,          // Không hoàn
            PolicyText = policyText,
            RefundPercentage = refundPercentage,
            StartTime = rental.StartTime.Value,
            TimeUntilStart = timeUntilStart
        };
        
        return ServiceResultDto<CancellationPolicyDto>.SuccessResult(
            policyDto, "Lấy thông tin chính sách thành công.");
    }
    catch (Exception ex)
    {
        return ServiceResultDto<CancellationPolicyDto>.FailureResult(
            $"Có lỗi xảy ra: {ex.Message}");
    }
}
```

### LƯU Ý QUAN TRỌNG:
- ✅ **Chỉ hoàn tiền thuê cơ bản (BasePrice)**
- ❌ **KHÔNG hoàn phí giữ chỗ (ReservationFee)**
- ❌ **KHÔNG hoàn phí cọc (DepositFee)**
- 📊 **Tỷ lệ hoàn tiền dựa trên thời gian còn lại đến StartTime**

---

## 6. DATABASE - PAYMENT RECORD CHO REFUND

### Tạo Payment record khi hủy đơn thành công

```csharp
// Trong PaymentService.CreateRefundRecordAsync()

public async Task<ServiceResultDto<Payment>> CreateRefundRecordAsync(
    int rentalId, decimal refundAmount)
{
    try
    {
        // Nếu không có tiền hoàn, không tạo record
        if (refundAmount <= 0)
        {
            return ServiceResultDto<Payment>.SuccessResult(
                null, "Không có tiền hoàn lại.");
        }
        
        var paymentRepo = _unitOfWork.GetRepository<Payment>();
        
        // Tạo payment record cho refund
        var refundPayment = new Payment
        {
            RentalId = rentalId,
            Amount = -refundAmount,  // ⚠️ SỐ ÂM để đánh dấu là refund
            Method = "refund",
            TransactionRef = $"REFUND-{rentalId}-{DateTime.Now.Ticks}",
            Status = "pending_refund",  // Chờ xử lý refund
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            IsDeleted = false
        };
        
        await paymentRepo.AddAsync(refundPayment);
        await _unitOfWork.SaveChangesAsync();
        
        return ServiceResultDto<Payment>.SuccessResult(
            refundPayment, "Tạo record hoàn tiền thành công.");
    }
    catch (Exception ex)
    {
        return ServiceResultDto<Payment>.FailureResult(
            $"Có lỗi xảy ra: {ex.Message}");
    }
}
```

### Các trạng thái của Payment.Status

| Status | Ý nghĩa | Khi nào |
|--------|---------|---------|
| `pending` | Chờ thanh toán | User chưa thanh toán |
| `paid` | Đã thanh toán | Thanh toán thành công |
| `failed` | Thanh toán thất bại | Gateway báo lỗi |
| `pending_refund` | Chờ hoàn tiền | Đơn bị hủy, chờ xử lý refund |
| `refunded` | Đã hoàn tiền | Đã chuyển tiền về user |

### Query để lấy các đơn cần refund

```csharp
// Trong Admin Dashboard hoặc background job
var pendingRefunds = await _paymentRepo
    .GetAllQueryable("RentalRecord,RentalRecord.Renter")
    .Where(p => p.Status == "pending_refund" && !p.IsDeleted)
    .OrderBy(p => p.CreateDate)
    .ToListAsync();
```

---

## 7. UI/UX CONSIDERATIONS

### Trang CancelRentalPolicy.cshtml - Cấu trúc gợi ý

```html
@page
@model CancelRentalPolicyModel
@{
    ViewData["Title"] = "Hủy đơn thuê xe";
}

<!-- Breadcrumb -->
<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="/Renter/MyTrips">Chuyến đi của tôi</a></li>
        <li class="breadcrumb-item active">Hủy đơn thuê</li>
    </ol>
</nav>

<div class="container mt-4">
    <h2>Hủy đơn thuê xe</h2>
    
    <!-- Thông tin đơn thuê -->
    <div class="card mb-4">
        <div class="card-header bg-info text-white">
            <h5>📋 THÔNG TIN ĐƠN THUÊ</h5>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-6">
                    <p><strong>Mã đơn:</strong> #@Model.Rental.Id</p>
                    <p><strong>Xe:</strong> @Model.Vehicle.Make @Model.Vehicle.Model</p>
                    <p><strong>Biển số:</strong> @Model.Vehicle.LicensePlate</p>
                </div>
                <div class="col-md-6">
                    <p><strong>Thời gian nhận xe:</strong> @Model.Rental.StartTime?.ToString("dd/MM/yyyy HH:mm")</p>
                    <p><strong>Thời gian trả xe:</strong> @Model.Rental.ExpectedEndTime?.ToString("dd/MM/yyyy HH:mm")</p>
                    <p><strong>Trạng thái:</strong> <span class="badge badge-warning">@Model.Rental.Status</span></p>
                </div>
            </div>
            <hr>
            <div class="row">
                <div class="col-md-6">
                    <p><strong>Giá thuê cơ bản:</strong> @Model.Rental.BasePrice.ToString("N0") VNĐ</p>
                    <p><strong>Phí giữ chỗ:</strong> @Model.Rental.ReservationFee.ToString("N0") VNĐ</p>
                    <p><strong>Phí cọc:</strong> @Model.Rental.DepositFee.ToString("N0") VNĐ</p>
                </div>
                <div class="col-md-6">
                    <p><strong>Tổng tiền:</strong> <span class="text-primary font-weight-bold">@Model.Rental.TotalPrice.ToString("N0") VNĐ</span></p>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Chính sách hủy đơn -->
    <div class="card mb-4">
        <div class="card-header bg-warning">
            <h5>📜 CHÍNH SÁCH HỦY ĐƠN</h5>
        </div>
        <div class="card-body">
            <h6>1. Điều kiện hủy đơn:</h6>
            <ul>
                <li>Chỉ được hủy đơn có trạng thái: <strong>Chờ Thanh Toán (Pending)</strong> hoặc <strong>Đã Xác Nhận (Confirmed)</strong></li>
                <li>Không thể hủy đơn đang thuê (Active) hoặc đã hoàn thành (Completed)</li>
            </ul>
            
            <h6>2. Chính sách hoàn tiền:</h6>
            <table class="table table-bordered">
                <thead class="thead-light">
                    <tr>
                        <th>Thời gian hủy</th>
                        <th>Số tiền hoàn lại</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Hủy trước <strong>24 giờ</strong></td>
                        <td><span class="text-success">✅ Hoàn 100% tiền thuê</span></td>
                    </tr>
                    <tr>
                        <td>Hủy từ <strong>12-24 giờ</strong> trước</td>
                        <td><span class="text-warning">⚠️ Hoàn 50% tiền thuê (phí hủy 50%)</span></td>
                    </tr>
                    <tr>
                        <td>Hủy dưới <strong>12 giờ</strong></td>
                        <td><span class="text-danger">❌ Không hoàn tiền thuê</span></td>
                    </tr>
                </tbody>
            </table>
            
            <div class="alert alert-warning">
                <h6>⚠️ LƯU Ý QUAN TRỌNG:</h6>
                <ul class="mb-0">
                    <li><strong>Phí giữ chỗ (ReservationFee):</strong> KHÔNG được hoàn lại</li>
                    <li><strong>Phí cọc (DepositFee):</strong> KHÔNG được hoàn lại</li>
                    <li><strong>Chỉ hoàn lại:</strong> Tiền thuê cơ bản (BasePrice) theo chính sách trên</li>
                </ul>
            </div>
            
            <h6>3. Thời gian xử lý:</h6>
            <ul>
                <li>Tiền sẽ được hoàn về tài khoản thanh toán ban đầu trong <strong>3-5 ngày làm việc</strong></li>
                <li>Bạn sẽ nhận được email thông báo khi refund được xử lý</li>
            </ul>
        </div>
    </div>
    
    <!-- Tính toán tiền hoàn lại -->
    <div class="card mb-4 @(Model.Policy.CanCancel ? "border-success" : "border-danger")">
        <div class="card-header @(Model.Policy.CanCancel ? "bg-success" : "bg-danger") text-white">
            <h5>💰 TÍNH TOÁN TIỀN HOÀN LẠI</h5>
        </div>
        <div class="card-body">
            @if (Model.Policy.CanCancel)
            {
                <p><strong>Thời gian còn lại đến giờ nhận xe:</strong> 
                    @((int)Model.Policy.TimeUntilStart.TotalHours) giờ @Model.Policy.TimeUntilStart.Minutes phút
                </p>
                <p><strong>Tỷ lệ hoàn tiền:</strong> <span class="badge badge-info">@Model.Policy.RefundPercentage</span></p>
                <p>@Model.Policy.PolicyText</p>
                
                <hr>
                
                <div class="row">
                    <div class="col-md-6">
                        <h6>Chi tiết hoàn tiền:</h6>
                        <table class="table table-sm">
                            <tr>
                                <td>Tiền thuê cơ bản:</td>
                                <td class="text-right">@Model.Rental.BasePrice.ToString("N0") VNĐ</td>
                            </tr>
                            <tr>
                                <td>Tỷ lệ hoàn:</td>
                                <td class="text-right">@Model.Policy.RefundPercentage</td>
                            </tr>
                            <tr class="font-weight-bold text-success">
                                <td>Số tiền được hoàn:</td>
                                <td class="text-right">@Model.Policy.RefundAmount.ToString("N0") VNĐ</td>
                            </tr>
                        </table>
                    </div>
                    <div class="col-md-6">
                        <h6>Không được hoàn lại:</h6>
                        <table class="table table-sm">
                            <tr>
                                <td>Phí giữ chỗ:</td>
                                <td class="text-right text-muted">@Model.Rental.ReservationFee.ToString("N0") VNĐ</td>
                            </tr>
                            <tr>
                                <td>Phí cọc:</td>
                                <td class="text-right text-muted">@Model.Rental.DepositFee.ToString("N0") VNĐ</td>
                            </tr>
                        </table>
                    </div>
                </div>
                
                <div class="alert alert-success mt-3">
                    <h5 class="mb-0">
                        <strong>Tổng tiền bạn sẽ nhận lại: @Model.Policy.RefundAmount.ToString("N0") VNĐ</strong>
                    </h5>
                </div>
            }
            else
            {
                <div class="alert alert-danger">
                    <p class="mb-0">@Model.Policy.PolicyText</p>
                </div>
            }
        </div>
    </div>
    
    <!-- Form xác nhận -->
    @if (Model.Policy.CanCancel)
    {
        <form method="post" id="cancelForm">
            <input type="hidden" name="rentalId" value="@Model.Rental.Id" />
            
            <div class="form-check mb-3">
                <input class="form-check-input" type="checkbox" id="agreeCheckbox" name="agreedToPolicy" value="true" required>
                <label class="form-check-label" for="agreeCheckbox">
                    <strong>Tôi đã đọc và đồng ý với chính sách hủy đơn</strong>
                </label>
            </div>
            
            <div class="d-flex justify-content-between">
                <a href="/Renter/MyTrips" class="btn btn-secondary">
                    <i class="fas fa-arrow-left"></i> Quay lại
                </a>
                <button type="submit" class="btn btn-danger" id="cancelBtn" disabled>
                    <i class="fas fa-times-circle"></i> Đồng ý và Hủy đơn
                </button>
            </div>
        </form>
    }
    else
    {
        <div class="text-center">
            <a href="/Renter/MyTrips" class="btn btn-primary">
                <i class="fas fa-arrow-left"></i> Quay lại
            </a>
        </div>
    }
</div>

@section Scripts {
    <script>
        $(document).ready(function() {
            // Enable/disable cancel button based on checkbox
            $('#agreeCheckbox').change(function() {
                $('#cancelBtn').prop('disabled', !this.checked);
            });
            
            // Confirm before submit
            $('#cancelForm').submit(function(e) {
                if (!confirm('Bạn có chắc chắn muốn hủy đơn thuê này không? Hành động này không thể hoàn tác.')) {
                    e.preventDefault();
                    return false;
                }
            });
        });
    </script>
}
```

### Responsive Design Considerations:
- ✅ Card layout cho mobile-friendly
- ✅ Table responsive cho danh sách chính sách
- ✅ Badge/Alert colors để nhấn mạnh thông tin quan trọng
- ✅ Icons (FontAwesome) để tăng UX
- ✅ Confirm dialog trước khi hủy

---

## 8. VALIDATION & ERROR HANDLING

### A. Frontend Validation

#### 1. Checkbox validation
```javascript
// Enable button chỉ khi checkbox được check
$('#agreeCheckbox').change(function() {
    $('#cancelBtn').prop('disabled', !this.checked);
});

// HTML5 required attribute
<input type="checkbox" required>
```

#### 2. Confirm dialog
```javascript
$('#cancelForm').submit(function(e) {
    if (!confirm('Bạn có chắc chắn muốn hủy đơn thuê này không?')) {
        e.preventDefault();
        return false;
    }
});
```

#### 3. Loading state
```javascript
$('#cancelForm').submit(function() {
    $('#cancelBtn')
        .prop('disabled', true)
        .html('<i class="fas fa-spinner fa-spin"></i> Đang xử lý...');
});
```

---

### B. Backend Validation

#### Trong `CancelRentalWithPolicyAsync()`:

```csharp
public async Task<ServiceResultDto<object>> CancelRentalWithPolicyAsync(
    int rentalId, int renterId, bool agreedToPolicy)
{
    try
    {
        // 1. Validate user đồng ý chính sách
        if (!agreedToPolicy)
        {
            return ServiceResultDto<object>.FailureResult(
                "Bạn phải đồng ý với chính sách hủy đơn.");
        }
        
        // 2. Lấy thông tin đơn thuê
        var rental = await GetRentalByIdAsync(rentalId);
        if (rental == null)
        {
            return ServiceResultDto<object>.FailureResult(
                "Không tìm thấy đơn thuê.");
        }
        
        // 3. Validate quyền hủy đơn
        if (rental.RenterId != renterId)
        {
            return ServiceResultDto<object>.FailureResult(
                "Bạn không có quyền hủy đơn này.");
        }
        
        // 4. Validate status
        if (rental.Status != RentalRecordStatus.Pending && 
            rental.Status != RentalRecordStatus.Confirmed)
        {
            return ServiceResultDto<object>.FailureResult(
                $"Chỉ có thể hủy đơn ở trạng thái Chờ Thanh Toán hoặc Đã Xác Nhận. " +
                $"Trạng thái hiện tại: {rental.Status}");
        }
        
        // 5. Validate thời gian (không quá StartTime)
        if (rental.StartTime < DateTime.Now)
        {
            return ServiceResultDto<object>.FailureResult(
                "Không thể hủy đơn đã quá thời gian bắt đầu.");
        }
        
        // 6. Tính tiền hoàn lại
        var policyResult = await GetCancellationPolicyAsync(rentalId, renterId);
        if (!policyResult.Success)
        {
            return ServiceResultDto<object>.FailureResult(policyResult.Message);
        }
        
        var policy = policyResult.Data;
        if (!policy.CanCancel)
        {
            return ServiceResultDto<object>.FailureResult(
                "Không thể hủy đơn thuê này theo chính sách.");
        }
        
        // 7. Cập nhật RentalRecord
        var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
        rental.Status = RentalRecordStatus.Cancelled;
        rental.UpdateDate = DateTime.Now;
        await rentalRepo.Update(rental);
        
        // 8. Tạo refund record
        if (policy.RefundAmount > 0)
        {
            var refundResult = await _paymentService.CreateRefundRecordAsync(
                rentalId, policy.RefundAmount);
            
            if (!refundResult.Success)
            {
                // Log error nhưng vẫn tiếp tục (có thể xử lý manual)
                Console.WriteLine($"[WARNING] Failed to create refund record: {refundResult.Message}");
            }
        }
        
        // 9. Cập nhật Vehicle
        var vehicle = await _vehicleService.GetVehicleByIdAsync(rental.VehicleId);
        if (vehicle != null)
        {
            vehicle.Status = VehicleStatus.Available;
            await _vehicleService.UpdateVehicleAsync(vehicle);
        }
        
        // 10. Commit transaction
        await _unitOfWork.SaveChangesAsync();
        
        // 11. Return success
        return ServiceResultDto<object>.SuccessResult(
            new { RefundAmount = policy.RefundAmount },
            $"Hủy đơn thành công. Số tiền hoàn lại: {policy.RefundAmount:N0} VNĐ");
    }
    catch (Exception ex)
    {
        // Log error
        Console.WriteLine($"[ERROR] CancelRentalWithPolicyAsync: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        
        return ServiceResultDto<object>.FailureResult(
            $"Có lỗi xảy ra khi hủy đơn: {ex.Message}");
    }
}
```

---

### C. Error Messages

| Tình huống | Message |
|------------|---------|
| Không check checkbox | "Bạn phải đồng ý với chính sách hủy đơn." |
| Không tìm thấy đơn | "Không tìm thấy đơn thuê." |
| Không có quyền | "Bạn không có quyền hủy đơn này." |
| Status không hợp lệ | "Chỉ có thể hủy đơn ở trạng thái Chờ Thanh Toán hoặc Đã Xác Nhận." |
| Quá thời gian | "Không thể hủy đơn đã quá thời gian bắt đầu." |
| Lỗi hệ thống | "Có lỗi xảy ra khi hủy đơn. Vui lòng thử lại sau." |

---

## 9. TESTING SCENARIOS

### Test Cases cần kiểm tra:

#### A. Happy Path (Thành công)

| # | Scenario | Expected Result |
|---|----------|-----------------|
| 1 | Hủy đơn Pending trước 24h | ✅ Hoàn 100% BasePrice |
| 2 | Hủy đơn Confirmed trước 24h | ✅ Hoàn 100% BasePrice |
| 3 | Hủy đơn Confirmed từ 12-24h | ✅ Hoàn 50% BasePrice |
| 4 | Hủy đơn Confirmed dưới 12h | ✅ Hoàn 0%, đơn vẫn bị hủy |
| 5 | Vehicle status = Available sau khi hủy | ✅ Status updated |
| 6 | Payment record được tạo đúng | ✅ Amount âm, status = pending_refund |

#### B. Validation (Thất bại)

| # | Scenario | Expected Error |
|---|----------|----------------|
| 7 | Hủy đơn Active | ❌ "Chỉ có thể hủy đơn Pending/Confirmed" |
| 8 | Hủy đơn Completed | ❌ "Chỉ có thể hủy đơn Pending/Confirmed" |
| 9 | Hủy đơn Cancelled | ❌ "Chỉ có thể hủy đơn Pending/Confirmed" |
| 10 | Hủy đơn đã quá StartTime | ❌ "Không thể hủy đơn đã quá thời gian bắt đầu" |
| 11 | User A hủy đơn của User B | ❌ "Bạn không có quyền hủy đơn này" |
| 12 | Không check checkbox đồng ý | ❌ "Bạn phải đồng ý với chính sách hủy đơn" |

#### C. Edge Cases

| # | Scenario | Expected Behavior |
|---|----------|-------------------|
| 13 | Hủy đơn có ReservationFee = 0 | ✅ Chỉ refund BasePrice |
| 14 | Hủy đơn có BasePrice = 0 | ✅ Không tạo refund record |
| 15 | Đúng 24h trước StartTime | ✅ Hoàn 100% |
| 16 | Đúng 12h trước StartTime | ✅ Hoàn 50% |
| 17 | 1 phút trước StartTime | ✅ Hoàn 0% |

---

### Test Data Example:

```csharp
// Test case: Hủy trước 24h → Hoàn 100%
var rental = new RentalRecord
{
    Id = 1,
    RenterId = 100,
    VehicleId = 10,
    StartTime = DateTime.Now.AddHours(25), // 25h sau
    Status = RentalRecordStatus.Confirmed,
    BasePrice = 500000,
    ReservationFee = 50000,
    DepositFee = 3000000
};

// Expected:
// - CanCancel = true
// - RefundAmount = 500000 (100%)
// - ReservationFee & DepositFee KHÔNG hoàn
```

---

## 10. TÓM TẮT FILES CẦN TẠO/SỬA

### Files cần TẠO MỚI:

| # | File Path | Type | Mô tả |
|---|-----------|------|-------|
| 1 | `BusinessLayer/DTOs/CancellationPolicyDto.cs` | DTO | Chứa thông tin chính sách & tiền hoàn |
| 2 | `EV Rental/Pages/Renter/CancelRentalPolicy.cshtml` | View | UI hiển thị chính sách hủy |
| 3 | `EV Rental/Pages/Renter/CancelRentalPolicy.cshtml.cs` | PageModel | Logic xử lý hủy đơn |

### Files cần CHỈNH SỬA:

| # | File Path | Thay đổi |
|---|-----------|----------|
| 1 | `BusinessLayer/Services/RentalService.cs` | Thêm `GetCancellationPolicyAsync()` & `CancelRentalWithPolicyAsync()` |
| 2 | `BusinessLayer/Services/PaymentService.cs` | Thêm `CreateRefundRecordAsync()` |
| 3 | `EV Rental/Pages/Renter/MyTrips.cshtml` | Đổi button "Hủy đơn" thành link |
| 4 | `EV Rental/Pages/Renter/MyTrips.cshtml.cs` | Xóa/deprecated `OnPostCancelRentalAsync()` |

### Tổng số files:
- **Tạo mới:** 3 files
- **Chỉnh sửa:** 4 files
- **Tổng:** 7 files

---

## 11. BONUS: CHUẨN BỊ CHO LUỒNG REFUND SAU NÀY

### A. Admin Dashboard - Quản lý Refund

#### Trang: `Admin/Payments/RefundManagement.cshtml`

**Chức năng:**
- Danh sách các Payment có `status = "pending_refund"`
- Hiển thị thông tin: RentalId, Amount, CreateDate, Renter info
- Button "Xử lý Refund" cho từng payment
- Filter theo ngày, trạng thái
- Pagination

**Query:**
```csharp
var pendingRefunds = await _paymentRepo
    .GetAllQueryable("RentalRecord,RentalRecord.Renter")
    .Where(p => p.Status == "pending_refund" && !p.IsDeleted)
    .OrderBy(p => p.CreateDate)
    .ToListAsync();
```

---

### B. Process Refund Logic

#### Method: `ProcessRefundAsync(int paymentId)`

**Các bước:**
1. Validate payment có status = "pending_refund"
2. Lấy thông tin payment & rental
3. **Tích hợp với Payment Gateway** (MoMo, VNPay, etc.) để refund
4. Nếu thành công:
   - Update payment.Status = "refunded"
   - Update payment.PaidAt = DateTime.Now
   - Gửi email thông báo cho user
5. Nếu thất bại:
   - Log error
   - Retry hoặc yêu cầu xử lý manual

```csharp
public async Task<ServiceResultDto<Payment>> ProcessRefundAsync(int paymentId)
{
    try
    {
        var payment = await GetPaymentByIdAsync(paymentId);
        
        if (payment.Status != "pending_refund")
            return ServiceResultDto<Payment>.FailureResult("Payment không ở trạng thái chờ refund.");
        
        // Tích hợp với gateway
        var refundResult = await _momoService.RefundAsync(
            payment.TransactionRef, 
            Math.Abs(payment.Amount));
        
        if (refundResult.Success)
        {
            payment.Status = "refunded";
            payment.PaidAt = DateTime.Now;
            await _paymentRepo.Update(payment);
            await _unitOfWork.SaveChangesAsync();
            
            // Send email
            await _emailService.SendRefundSuccessEmailAsync(payment);
            
            return ServiceResultDto<Payment>.SuccessResult(payment, "Refund thành công.");
        }
        else
        {
            return ServiceResultDto<Payment>.FailureResult($"Refund thất bại: {refundResult.Message}");
        }
    }
    catch (Exception ex)
    {
        return ServiceResultDto<Payment>.FailureResult($"Lỗi: {ex.Message}");
    }
}
```

---

### C. Email Notifications

#### 1. Email khi đơn bị hủy
```
Subject: Đơn thuê xe #123 đã được hủy

Xin chào [Tên khách hàng],

Đơn thuê xe #123 của bạn đã được hủy thành công.

Thông tin hoàn tiền:
- Số tiền hoàn lại: 500,000 VNĐ
- Phương thức hoàn: Về tài khoản thanh toán ban đầu
- Thời gian xử lý: 3-5 ngày làm việc

Chúng tôi sẽ gửi email thông báo khi refund được xử lý.

Trân trọng,
EV Rental Team
```

#### 2. Email khi refund thành công
```
Subject: Hoàn tiền thành công cho đơn #123

Xin chào [Tên khách hàng],

Khoản hoàn tiền cho đơn thuê xe #123 đã được xử lý thành công.

Chi tiết:
- Số tiền: 500,000 VNĐ
- Thời gian xử lý: 15/11/2025 14:30
- Phương thức: Hoàn về tài khoản MoMo

Vui lòng kiểm tra tài khoản của bạn.

Trân trọng,
EV Rental Team
```

---

### D. Background Job (Optional)

Sử dụng **Hangfire** hoặc **Quartz.NET** để tự động xử lý refund:

```csharp
// Chạy mỗi 1 giờ
[AutomaticRetry(Attempts = 3)]
public async Task ProcessPendingRefundsJob()
{
    var pendingRefunds = await _paymentService.GetPendingRefundsAsync();
    
    foreach (var payment in pendingRefunds)
    {
        try
        {
            await _paymentService.ProcessRefundAsync(payment.Id);
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"[ERROR] Failed to process refund {payment.Id}: {ex.Message}");
        }
    }
}
```

---

## 📊 DIAGRAM TỔNG QUAN

```
┌─────────────────────────────────────────────────────────────┐
│                     USER CANCEL FLOW                        │
└─────────────────────────────────────────────────────────────┘

[MyTrips Page]
      │
      ├─ Click "Hủy đơn" button
      │
      ↓
[CancelRentalPolicy Page]
      │
      ├─ Load policy & calculate refund
      │
      ├─ User reads policy
      │
      ├─ User checks "Agree" checkbox
      │
      ├─ User clicks "Đồng ý và Hủy đơn"
      │
      ↓
[Backend Processing]
      │
      ├─ Validate (status, permission, time, agreement)
      │
      ├─ Calculate refund amount
      │
      ├─ Update RentalRecord.Status = Cancelled
      │
      ├─ Create Payment (status = "pending_refund")
      │
      ├─ Update Vehicle.Status = Available
      │
      ↓
[Success Response]
      │
      └─ Redirect to MyTrips with success message


┌─────────────────────────────────────────────────────────────┐
│                  ADMIN REFUND FLOW (Future)                 │
└─────────────────────────────────────────────────────────────┘

[Admin Dashboard]
      │
      ├─ View list of "pending_refund" payments
      │
      ├─ Click "Xử lý Refund" for a payment
      │
      ↓
[Backend Processing]
      │
      ├─ Call Payment Gateway API (MoMo/VNPay refund)
      │
      ├─ If success:
      │     ├─ Update Payment.Status = "refunded"
      │     ├─ Update Payment.PaidAt = DateTime.Now
      │     └─ Send email to user
      │
      └─ If failed: Log error & retry/manual
```

---

## 🎯 KẾT LUẬN

### Ưu điểm của thiết kế này:

1. ✅ **Minh bạch:** User biết rõ chính sách trước khi hủy
2. ✅ **Linh hoạt:** Dễ điều chỉnh rule chính sách (thay đổi 24h → 48h chỉ cần sửa 1 chỗ)
3. ✅ **Tách biệt logic:** Hủy đơn ≠ Refund tiền → Dễ quản lý
4. ✅ **Audit trail:** Có Payment record để track refund
5. ✅ **UX tốt:** Hiển thị rõ ràng số tiền được hoàn
6. ✅ **Scalable:** Dễ mở rộng thêm tính năng sau này

### Các bước tiếp theo:

1. 🔨 Tạo `CancellationPolicyDto.cs`
2. 🔨 Thêm methods vào `RentalService.cs` & `PaymentService.cs`
3. 🎨 Tạo UI `CancelRentalPolicy.cshtml`
4. 🎨 Sửa `MyTrips.cshtml` (đổi button thành link)
5. ✅ Test toàn bộ flow
6. 📧 (Optional) Thêm email notification
7. 🔮 (Future) Xây dựng Admin Refund Management

---

## 📞 LƯU Ý KHI IMPLEMENT

### 1. Database Transaction
- Sử dụng transaction để đảm bảo data consistency
- Nếu bất kỳ bước nào fail → rollback toàn bộ

### 2. Error Logging
- Log đầy đủ các error để debug
- Đặc biệt là phần tạo refund record

### 3. Security
- Validate user permission ở cả frontend & backend
- Không tin tưởng data từ client

### 4. Performance
- Cache policy rules nếu cần
- Index cho Payment.Status để query nhanh

### 5. Testing
- Unit test cho logic tính tiền
- Integration test cho toàn bộ flow
- UI test cho checkbox & button behavior

---

**Tài liệu này được tạo để hướng dẫn implement tính năng Hủy đơn thuê xe có chính sách hoàn tiền.**

**Nếu có thắc mắc, vui lòng tham khảo lại các section tương ứng.**

**Good luck! 🚀**
