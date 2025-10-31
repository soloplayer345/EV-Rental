using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class UpdateVehicleModel : PageModel
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVehicleModel(IVehicleRepo vehicleRepo, IWebHostEnvironment env, IUnitOfWork unitOfWork)
        {
            _vehicleRepo = vehicleRepo;
            _env = env;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = new();

        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (Vehicle == null) return RedirectToPage("Index");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Image != null)
            {
                var fileName = $"{Guid.NewGuid()}_{Image.FileName}";
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await Image.CopyToAsync(stream);
                }

                Vehicle.ImageUrl = "/uploads/" + fileName;
            }

            await _vehicleRepo.Update(Vehicle);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
