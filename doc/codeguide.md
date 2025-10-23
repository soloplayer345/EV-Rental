# Hướng Dẫn Code - EV Rental Project

## 📋 Mục Lục
1. [Tổng Quan Kiến Trúc](#tổng-quan-kiến-trúc)
2. [Cấu Trúc Dự Án](#cấu-trúc-dự-án)
3. [Quy Tắc Code](#quy-tắc-code)
4. [Hướng Dẫn Làm Việc Với Từng Layer](#hướng-dẫn-làm-việc-với-từng-layer)
5. [Database & Migrations](#database--migrations)
6. [Authentication & Authorization](#authentication--authorization)
7. [Best Practices](#best-practices)
8. [Checklist Trước Khi Commit](#checklist-trước-khi-commit)

---

## 🏗️ Tổng Quan Kiến Trúc

Project sử dụng **3-Layer Architecture**:

```
PresentationLayer (EV Rental)
        ↓
BusinessLayer
        ↓
DataAccessLayer
```

### Nguyên Tắc Phân Tầng:
- **PresentationLayer**: Xử lý UI, Razor Pages, Controllers
- **BusinessLayer**: Business Logic, Services, DTOs, Mapping
- **DataAccessLayer**: Database, Entities, Repositories, Migrations

⚠️ **QUAN TRỌNG**: 
- Presentation KHÔNG được gọi trực tiếp DataAccessLayer
- DataAccessLayer KHÔNG được reference đến BusinessLayer
- BusinessLayer là cầu nối duy nhất giữa hai layer

---

## 📁 Cấu Trúc Dự Án

### DataAccessLayer/
```
DataAccessLayer/
├── Entities/              # Entity classes (models)
│   ├── Account.cs
│   ├── Vehicle.cs
│   ├── RentalRecord.cs
│   └── ...
├── Enums/                 # Enum types
│   ├── AccountRole.cs
│   ├── AccountStatus.cs
│   └── ...
├── Repositories/          # Repository pattern implementation
│   ├── GenericRepo.cs
│   ├── AccountRepo.cs
│   └── ...
├── Interfaces/            # Repository interfaces
│   ├── IGenericRepo.cs
│   ├── IAccountRepo.cs
│   └── IUnitOfWork.cs
├── Migrations/            # EF Core migrations
├── EVRentalDBContext.cs   # DbContext
├── DataSeeder.cs          # Seed data
└── UnitOfWork.cs          # Unit of Work pattern
```

### BusinessLayer/
```
BusinessLayer/
├── Services/              # Business logic services
│   └── AuthService.cs
├── Interfaces/            # Service interfaces
│   └── IAuthService.cs
├── DTOs/                  # Data Transfer Objects
│   ├── LoginRequestDto.cs
│   ├── RegisterRequestDto.cs
│   └── ServiceResultDto.cs
└── Mapping/               # DTO ↔ Entity mapping
    └── AuthMapper.cs
```

### PresentationLayer (EV Rental)/
```
EV Rental/
├── Pages/                 # Razor Pages
│   ├── Auth/
│   ├── Admin/
│   └── ...
├── Helpers/               # Helper classes
│   └── SessionHelper.cs
├── Middlewares/           # Custom middlewares
│   └── RoleBasedRedirectMiddleware.cs
├── wwwroot/               # Static files (CSS, JS, images)
└── Program.cs             # App configuration
```

---

## 📝 Quy Tắc Code

### 1. Naming Conventions

#### C# Classes & Methods
```csharp
// Classes: PascalCase
public class AccountService { }

// Methods: PascalCase
public async Task<ServiceResultDto> RegisterAsync() { }

// Private fields: _camelCase
private readonly IAccountRepo _accountRepo;

// Properties: PascalCase
public string Email { get; set; }

// Local variables: camelCase
var userName = "admin";

// Constants: PascalCase
public const int MaxLoginAttempts = 5;
```

#### Files & Folders
- **Files**: PascalCase với extension `.cs`
  - `AuthService.cs`, `LoginRequestDto.cs`
- **Folders**: PascalCase
  - `Services/`, `DTOs/`, `Entities/`

### 2. Async/Await Pattern
```csharp
// ✅ ĐÚNG: Sử dụng Async suffix và await
public async Task<Account> GetAccountByIdAsync(int id)
{
    return await _context.Accounts.FindAsync(id);
}

// ❌ SAI: Không có Async suffix
public async Task<Account> GetAccountById(int id)
{
    return await _context.Accounts.FindAsync(id);
}
```

### 3. Nullable Reference Types
```csharp
// ✅ ĐÚNG: Khai báo nullable rõ ràng
public string? Email { get; set; }  // Có thể null
public string Password { get; set; } = string.Empty; // Không null

// ❌ SAI: Không xử lý null
public string Email { get; set; }
```

### 4. Error Handling
```csharp
// ✅ ĐÚNG: Try-catch với logging
try
{
    var result = await _service.DoSomethingAsync();
    return result;
}
catch (Exception ex)
{
    // Log error
    return new ServiceResultDto 
    { 
        Success = false, 
        Message = "Có lỗi xảy ra" 
    };
}

// ❌ SAI: Không xử lý exception
var result = await _service.DoSomethingAsync();
return result;
```

---

## 🔧 Hướng Dẫn Làm Việc Với Từng Layer

### A. DataAccessLayer

#### 1. Tạo Entity Mới
```csharp
// Entities/NewEntity.cs
public class NewEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Status { get; set; }
    
    // Navigation properties
    public Account? Account { get; set; }
    public int AccountId { get; set; }
}
```

#### 2. Thêm Entity vào DbContext
```csharp
// EVRentalDBContext.cs
public DbSet<NewEntity> NewEntities { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    modelBuilder.Entity<NewEntity>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        
        // Relationships
        entity.HasOne(e => e.Account)
              .WithMany()
              .HasForeignKey(e => e.AccountId);
    });
}
```

#### 3. Tạo Repository
```csharp
// Interfaces/INewEntityRepo.cs
public interface INewEntityRepo : IGenericRepo<NewEntity>
{
    Task<NewEntity?> GetByNameAsync(string name);
}

// Repositories/NewEntityRepo.cs
public class NewEntityRepo : GenericRepo<NewEntity>, INewEntityRepo
{
    public NewEntityRepo(EVRentalDBContext context) : base(context) { }
    
    public async Task<NewEntity?> GetByNameAsync(string name)
    {
        return await _context.NewEntities
            .FirstOrDefaultAsync(e => e.Name == name);
    }
}
```

#### 4. Thêm vào UnitOfWork
```csharp
// Interfaces/IUnitOfWork.cs
public interface IUnitOfWork
{
    INewEntityRepo NewEntityRepo { get; }
    Task<int> SaveChangesAsync();
}

// UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    private readonly EVRentalDBContext _context;
    
    public INewEntityRepo NewEntityRepo { get; }
    
    public UnitOfWork(EVRentalDBContext context)
    {
        _context = context;
        NewEntityRepo = new NewEntityRepo(context);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
```

### B. BusinessLayer

#### 1. Tạo DTOs
```csharp
// DTOs/NewEntityRequestDto.cs
public class NewEntityRequestDto
{
    public string Name { get; set; } = string.Empty;
    public int Status { get; set; }
}

// DTOs/NewEntityResponseDto.cs
public class NewEntityResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Status { get; set; }
}
```

#### 2. Tạo Mapper
```csharp
// Mapping/NewEntityMapper.cs
public static class NewEntityMapper
{
    public static NewEntity ToEntity(this NewEntityRequestDto dto)
    {
        return new NewEntity
        {
            Name = dto.Name,
            Status = dto.Status,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now
        };
    }
    
    public static NewEntityResponseDto ToDto(this NewEntity entity)
    {
        return new NewEntityResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Status = entity.Status
        };
    }
}
```

#### 3. Tạo Service
```csharp
// Interfaces/INewEntityService.cs
public interface INewEntityService
{
    Task<ServiceResultDto> CreateAsync(NewEntityRequestDto request);
    Task<NewEntityResponseDto?> GetByIdAsync(int id);
}

// Services/NewEntityService.cs
public class NewEntityService : INewEntityService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public NewEntityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ServiceResultDto> CreateAsync(NewEntityRequestDto request)
    {
        try
        {
            var entity = request.ToEntity();
            await _unitOfWork.NewEntityRepo.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            
            return new ServiceResultDto 
            { 
                Success = true, 
                Message = "Tạo thành công" 
            };
        }
        catch (Exception ex)
        {
            return new ServiceResultDto 
            { 
                Success = false, 
                Message = $"Lỗi: {ex.Message}" 
            };
        }
    }
    
    public async Task<NewEntityResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.NewEntityRepo.GetByIdAsync(id);
        return entity?.ToDto();
    }
}
```

#### 4. Đăng Ký Service trong Program.cs
```csharp
// Program.cs
builder.Services.AddScoped<INewEntityService, NewEntityService>();
```

### C. PresentationLayer

#### 1. Tạo Razor Page
```csharp
// Pages/NewEntity/Create.cshtml.cs
public class CreateModel : PageModel
{
    private readonly INewEntityService _service;
    
    [BindProperty]
    public NewEntityRequestDto Request { get; set; } = new();
    
    public CreateModel(INewEntityService service)
    {
        _service = service;
    }
    
    public void OnGet()
    {
        // Load initial data
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        var result = await _service.CreateAsync(Request);
        
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToPage("./Index");
        }
        
        ModelState.AddModelError(string.Empty, result.Message);
        return Page();
    }
}
```

#### 2. Sử Dụng Session Helper
```csharp
// Lưu session
SessionHelper.SetObjectAsJson(HttpContext.Session, "UserInfo", userDto);

// Đọc session
var user = SessionHelper.GetObjectFromJson<UserDto>(HttpContext.Session, "UserInfo");

// Xóa session
HttpContext.Session.Remove("UserInfo");
```

---

## 🗄️ Database & Migrations

### 1. Tạo Migration Mới
```bash
# Di chuyển đến thư mục DataAccessLayer
cd DataAccessLayer

# Tạo migration
dotnet ef migrations add MigrationName --startup-project ../EV\ Rental/PresentaionLayer.csproj

# Áp dụng migration
dotnet ef database update --startup-project ../EV\ Rental/PresentaionLayer.csproj
```

### 2. Seed Data
```csharp
// DataSeeder.cs
public static void SeedData(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Account>().HasData(
        new Account
        {
            Id = 1,
            Email = "admin@evrental.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = AccountRole.Admin,
            Status = AccountStatus.Active,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now
        }
    );
}
```

### 3. Connection String
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EVRentalDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

---

## 🔐 Authentication & Authorization

### 1. Login Flow
```
User Input → LoginRequestDto → AuthService.LoginAsync()
    ↓
Validate Credentials
    ↓
Generate Token/Session
    ↓
Return AuthResponseDto
    ↓
Store in Session (SessionHelper)
    ↓
Redirect based on Role
```

### 2. Role-Based Access
```csharp
// Sử dụng trong Razor Page
public class AdminPageModel : PageModel
{
    public IActionResult OnGet()
    {
        var user = SessionHelper.GetObjectFromJson<AuthResponseDto>(
            HttpContext.Session, "UserInfo");
            
        if (user == null || user.Role != AccountRole.Admin)
        {
            return RedirectToPage("/Auth/Login");
        }
        
        return Page();
    }
}
```

### 3. Middleware
```csharp
// RoleBasedRedirectMiddleware.cs
// Tự động redirect user dựa trên role
// Admin → /Admin/Dashboard
// Staff → /Staff/Dashboard
// Renter → /Renter/Dashboard
```

---

## ✅ Best Practices

### 1. Repository Pattern
```csharp
// ✅ ĐÚNG: Sử dụng repository thông qua UnitOfWork
var account = await _unitOfWork.AccountRepo.GetByEmailAsync(email);
await _unitOfWork.SaveChangesAsync();

// ❌ SAI: Truy cập DbContext trực tiếp
var account = await _context.Accounts.FindAsync(id);
await _context.SaveChangesAsync();
```

### 2. DTO Usage
```csharp
// ✅ ĐÚNG: Sử dụng DTO cho data transfer
public async Task<ServiceResultDto> CreateAccount(RegisterRequestDto request)
{
    var account = request.ToEntity();
    // ...
}

// ❌ SAI: Expose Entity trực tiếp
public async Task<Account> CreateAccount(Account account)
{
    // ...
}
```

### 3. Password Hashing
```csharp
// ✅ ĐÚNG: Hash password trước khi lưu
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

// Verify password
var isValid = BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);

// ❌ SAI: Lưu plain text
account.Password = request.Password;
```

### 4. Query Optimization
```csharp
// ✅ ĐÚNG: Eager loading
var account = await _context.Accounts
    .Include(a => a.Renter)
    .ThenInclude(r => r.RentalRecords)
    .FirstOrDefaultAsync(a => a.Id == id);

// ❌ SAI: N+1 query problem
var account = await _context.Accounts.FindAsync(id);
var renter = await _context.Renters.FindAsync(account.RenterId);
```

### 5. Validation
```csharp
// ✅ ĐÚNG: Validate ở nhiều layer
// 1. Client-side (HTML5)
<input type="email" required />

// 2. Data Annotations
[Required(ErrorMessage = "Email là bắt buộc")]
[EmailAddress(ErrorMessage = "Email không hợp lệ")]
public string Email { get; set; }

// 3. Business Logic
if (await _unitOfWork.AccountRepo.EmailExistsAsync(email))
{
    return new ServiceResultDto 
    { 
        Success = false, 
        Message = "Email đã tồn tại" 
    };
}
```

---

## 📋 Checklist Trước Khi Commit

### Code Quality
- [ ] Code đã format đúng convention
- [ ] Không có warning hoặc error
- [ ] Đã test chức năng mới
- [ ] Đã xử lý exception properly
- [ ] Password đã được hash (không plain text)

### Architecture
- [ ] Tuân thủ 3-layer architecture
- [ ] Sử dụng Repository pattern
- [ ] Sử dụng DTO cho data transfer
- [ ] Service đã được đăng ký DI

### Database
- [ ] Migration đã được tạo (nếu thay đổi schema)
- [ ] Seed data đã được cập nhật (nếu cần)
- [ ] Relationship đã được config đúng

### Documentation
- [ ] Comment cho code phức tạp
- [ ] Update README.md (nếu cần)
- [ ] Update API documentation (nếu có)

### Git
- [ ] Branch name có ý nghĩa: `feature/add-vehicle-management`
- [ ] Commit message rõ ràng: `feat: add vehicle CRUD operations`
- [ ] Không commit file sensitive (connection string, credentials)
- [ ] Đã pull code mới nhất trước khi push

---

## 🤖 Hướng Dẫn Cho AI Assistant

### Khi Được Yêu Cầu Tạo Feature Mới:

1. **Phân tích yêu cầu**: Xác định entities, business logic, UI cần thiết
2. **Thứ tự implement**:
   - DataAccessLayer: Entity → Repository → UnitOfWork
   - BusinessLayer: DTO → Mapper → Service
   - PresentationLayer: Razor Page → View
3. **Tạo migration** nếu có thay đổi database
4. **Test** từng layer riêng biệt

### Pattern Cần Tuân Thủ:
- Repository Pattern
- Unit of Work Pattern
- DTO Pattern
- Dependency Injection
- Async/Await

### Không Được:
- Bỏ qua bất kỳ layer nào
- Truy cập DbContext trực tiếp từ Presentation
- Lưu password dạng plain text
- Hardcode connection string
- Ignore exception handling

---

## 📞 Liên Hệ & Hỗ Trợ

Nếu có thắc mắc về:
- **Architecture**: Hỏi team lead
- **Database**: Check file `EVRentalDB.sql` trong thư mục `doc/`
- **Conventions**: Đọc lại section "Quy Tắc Code"

---

**Version**: 1.0  
**Last Updated**: 24/10/2025  
**Maintained by**: EV Rental Development Team
