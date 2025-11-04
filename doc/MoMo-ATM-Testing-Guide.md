# 🏧 Hướng Dẫn Test Thanh Toán MoMo ATM

## 📋 Mục Lục
- [Giới Thiệu](#giới-thiệu)
- [Thông Tin Test Environment](#thông-tin-test-environment)
- [Thông Tin Thẻ ATM Test](#thông-tin-thẻ-atm-test)
- [Các Bước Test Thanh Toán](#các-bước-test-thanh-toán)
- [Các Trường Hợp Test](#các-trường-hợp-test)
- [Kết Quả Mong Đợi](#kết-quả-mong-đợi)

---

## 🎯 Giới Thiệu

Tài liệu này hướng dẫn cách test thanh toán MoMo sử dụng phương thức **ATM/Internet Banking** trong môi trường **Sandbox** (Test Environment).

---

## 🔧 Thông Tin Test Environment

### **MoMo Sandbox Configuration**
```json
{
  "PartnerCode": "MOMOBKUN20180529",
  "AccessKey": "klm05TvNBzhg7h7j",
  "SecretKey": "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa",
  "ReturnUrl": "https://localhost:7158/Payment/MoMoReturn",
  "IpnUrl": "https://localhost:7158/Payment/MoMoIPN",
  "RequestType": "payWithATM"
}
```

### **API Endpoint**
- **URL**: `https://test-payment.momo.vn/v2/gateway/api/create`
- **Environment**: Sandbox (Test)

---

## 💳 Thông Tin Thẻ ATM Test

MoMo cung cấp các thẻ ATM test để mô phỏng các trường hợp thanh toán:

### **1. Thẻ Test Thành Công ✅**

#### **Ngân hàng NCB (Ngân hàng Quốc Dân)**
```
Số thẻ:        9704 0000 0000 0018
Tên chủ thẻ:   NGUYEN VAN A
Ngày hết hạn:  03/07
OTP:           123456
```

#### **Ngân hàng VCB (Vietcombank)**
```
Số thẻ:        9704198526191432198
Tên chủ thẻ:   NGUYEN VAN A
Ngày hết hạn:  03/07
OTP:           123456
```

#### **Ngân hàng ACB**
```
Số thẻ:        9704 0000 0000 0001
Tên chủ thẻ:   NGUYEN VAN A
Ngày hết hạn:  03/07
OTP:           123456
```

---

### **2. Thẻ Test Thất Bại ❌**

#### **Tài khoản không đủ tiền**
```
Số thẻ:        9704 0000 0000 0026
Tên chủ thẻ:   NGUYEN VAN A
Ngày hết hạn:  03/07
Kết quả:       Lỗi "Tài khoản không đủ số dư"
```

#### **Thẻ bị khóa**
```
Số thẻ:        9704 0000 0000 0034
Tên chủ thẻ:   NGUYEN VAN A
Ngày hết hạn:  03/07
Kết quả:       Lỗi "Thẻ/Tài khoản bị khóa"
```

#### **Thẻ hết hạn**
```
Số thẻ:        9704 0000 0000 0042
Tên chủ thẻ:   NGUYEN VAN A
Ngày hết hạn:  03/07
Kết quả:       Lỗi "Thẻ đã hết hạn"
```

---

## 🧪 Các Bước Test Thanh Toán

### **Bước 1: Chọn xe và xác nhận thuê**
1. Đăng nhập vào hệ thống với tài khoản **Renter**
2. Truy cập trang danh sách xe: `/Renter/VehicleList`
3. Chọn một xe và click **"Thuê Xe"**
4. Điền thông tin:
   - Thời gian nhận xe
   - Thời gian trả xe
   - Trạm nhận xe
   - Trạm trả xe
5. Click **"Xác Nhận Thuê Xe"**

---

### **Bước 2: Chọn phương thức thanh toán**
1. Trên trang xác nhận, chọn **"Thanh toán online"**
2. Chọn **"Thanh toán qua MoMo"**
3. Click **"Thanh Toán"**
4. Hệ thống sẽ tạo đơn thuê với trạng thái **Pending** và chuyển hướng đến MoMo

---

### **Bước 3: Thanh toán trên MoMo Gateway**
1. Tại trang MoMo Test Payment, chọn **"Thanh toán bằng thẻ ATM"**
2. Nhập thông tin thẻ ATM test:
   - Số thẻ: `9704 0000 0000 0018` (NCB)
   - Tên chủ thẻ: `NGUYEN VAN A`
   - Ngày hết hạn: `03/07`
3. Click **"Tiếp tục"**
4. Nhập mã OTP: `123456`
5. Click **"Xác nhận thanh toán"**

---

### **Bước 4: Xác nhận kết quả**
1. Sau khi thanh toán thành công, MoMo sẽ redirect về: `/Payment/MoMoReturn`
2. Hệ thống sẽ:
   - ✅ Xác nhận đơn thuê (status: **Confirmed**)
   - ✅ Cập nhật trạng thái xe (status: **Rented**)
   - ✅ Tạo bản ghi thanh toán
   - ✅ **Gửi email chứa mã OTP** đến email của người thuê
3. Redirect đến trang **Payment Success** hiển thị:
   - Thông tin giao dịch
   - Mã đơn hàng
   - Số tiền đã thanh toán
   - Mã OTP để nhận xe

---

## 📝 Các Trường Hợp Test

### **Test Case 1: Thanh toán thành công ✅**
```
Input:
  - Thẻ ATM: 9704 0000 0000 0018
  - OTP: 123456
  
Expected Output:
  - ResultCode: 0
  - Message: "Successful" hoặc "Giao dịch thành công"
  - Rental Status: Confirmed
  - Vehicle Status: Rented
  - Payment Record: Created
  - Email: Sent với mã OTP
```

---

### **Test Case 2: Tài khoản không đủ tiền ❌**
```
Input:
  - Thẻ ATM: 9704 0000 0000 0026
  - OTP: 123456
  
Expected Output:
  - ResultCode: 1000 (hoặc 51)
  - Message: "Tài khoản không đủ số dư"
  - Rental Status: Pending (không thay đổi)
  - Vehicle Status: Available (không thay đổi)
  - Redirect: /Payment/PaymentFailure
```

---

### **Test Case 3: Thẻ bị khóa ❌**
```
Input:
  - Thẻ ATM: 9704 0000 0000 0034
  - OTP: 123456
  
Expected Output:
  - ResultCode: 1000 (hoặc 12)
  - Message: "Thẻ/Tài khoản bị khóa"
  - Rental Status: Pending (không thay đổi)
  - Vehicle Status: Available (không thay đổi)
```

---

### **Test Case 4: Người dùng hủy thanh toán ❌**
```
Action:
  - Vào trang MoMo Payment
  - Click "Hủy giao dịch" hoặc đóng trang
  
Expected Output:
  - ResultCode: 1006 (hoặc 24)
  - Message: "Giao dịch bị hủy"
  - Rental Status: Pending (có thể xóa sau timeout)
  - Vehicle Status: Available
```

---

## 🎯 Kết Quả Mong Đợi

### **✅ Thanh Toán Thành Công**

#### **1. Database Changes**
```sql
-- RentalRecords table
Status = 1 (Confirmed)
OtpCode = "123456" (6 chữ số)

-- Vehicles table
Status = 1 (Rented)

-- Payments table
INSERT new record với:
  - RentalId
  - Amount
  - Method = "momo"
  - TransactionRef = OrderId từ MoMo
  - Status = "Completed"
```

#### **2. Email Sent 📧**
Email được gửi tới người thuê xe với nội dung:
- ✅ Tiêu đề: "🔋 Mã OTP #[RentalId] - Thanh Toán Thành Công - EV Rental"
- ✅ Mã OTP hiển thị rõ ràng (6 chữ số)
- ✅ Thông tin đơn thuê: Xe, trạm, thời gian
- ✅ Số tiền đã thanh toán
- ✅ Hướng dẫn nhận xe

#### **3. Success Page Display**
Trang `/Payment/PaymentSuccess` hiển thị:
- ✅ Transaction ID từ MoMo
- ✅ Order ID (BOOKING[VehicleId]_[Timestamp])
- ✅ Payment Method: "Ví MoMo"
- ✅ Amount: Số tiền đã thanh toán
- ✅ Rental ID
- ✅ Vehicle ID
- ✅ **OTP Code** (để nhận xe)
- ✅ Start Time & End Time

---

### **❌ Thanh Toán Thất Bại**

#### **1. Database Changes**
```sql
-- RentalRecords table
Status = 0 (Pending) - Không thay đổi

-- Vehicles table
Status = 0 (Available) - Không thay đổi

-- Payments table
Không tạo bản ghi mới
```

#### **2. Failure Page Display**
Trang `/Payment/PaymentFailure` hiển thị:
- ❌ Error Message từ MoMo
- ❌ Transaction ID (nếu có)
- ❌ Order ID
- ❌ Response Code
- ⚠️ Hướng dẫn thử lại

---

## 🔍 Debug & Troubleshooting

### **Kiểm tra Console Logs**
```csharp
// Các log quan trọng trong MoMoReturn.cshtml.cs
[SUCCESS] Payment successful, confirming rental...
[SESSION] PendingRentalId found: True/False
[RENTAL] Confirming rental #[RentalId]...
[RENTAL] Result: True/False, Message: [Message]
[PAYMENT] Payment record created for rental #[RentalId]
[EMAIL] OTP email sent to [Email]
[SUCCESS] Redirecting to success page
```

### **Kiểm tra Session**
```csharp
// Session key format
"PendingRentalId_{OrderId}"

// Ví dụ
"PendingRentalId_BOOKING5_20251104030216"
```

### **Verify Signature**
```csharp
// MoMo signature verification
// Kiểm tra signature có match với SecretKey không
var isValidSignature = rawHash == signature;
```

---

## 📞 Hỗ Trợ

### **MoMo Sandbox Support**
- **Documentation**: https://developers.momo.vn/
- **Support Email**: support@momo.vn
- **Test Cards**: Sử dụng các số thẻ test được cung cấp ở trên

### **Project Support**
- **Issue Tracker**: GitHub Issues
- **Email**: thuongphong1625@gmail.com

---

## 📌 Lưu Ý Quan Trọng

1. ⚠️ **Chỉ test trên Sandbox**: Đừng sử dụng thông tin thẻ thật
2. ⚠️ **OTP luôn là**: `123456` cho tất cả thẻ test
3. ⚠️ **Session timeout**: 30 phút - Đơn pending sẽ bị hủy nếu không thanh toán
4. ⚠️ **HTTPS required**: MoMo callback chỉ hoạt động trên HTTPS
5. ⚠️ **Email configuration**: Kiểm tra SMTP settings trong `appsettings.json`
6. ⚠️ **Whitelist payment routes**: Middleware phải cho phép `/payment` routes

---

## ✅ Checklist Test

- [ ] Test thanh toán thành công với thẻ NCB
- [ ] Test thanh toán thành công với thẻ VCB
- [ ] Test tài khoản không đủ tiền
- [ ] Test thẻ bị khóa
- [ ] Test người dùng hủy giao dịch
- [ ] Verify rental status changed to Confirmed
- [ ] Verify vehicle status changed to Rented
- [ ] Verify payment record created
- [ ] **Verify OTP email sent successfully** ✉️
- [ ] Verify success page displays correctly
- [ ] Verify failure page displays correctly
- [ ] Check console logs for errors

---

**📅 Last Updated**: November 4, 2025  
**🔖 Version**: 1.0  
**✍️ Author**: EV Rental Development Team
