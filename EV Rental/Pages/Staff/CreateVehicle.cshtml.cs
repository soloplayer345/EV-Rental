using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class CreateVehicleModel : PageModel
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IWebHostEnvironment _env;

        public CreateVehicleModel(IVehicleRepo vehicleRepo, IWebHostEnvironment env)
        {
            _vehicleRepo = vehicleRepo;
            _env = env;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (ImageFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{ImageFile.FileName}";
                var path = Path.Combine(_env.WebRootPath, "uploads", fileName);

                using var stream = System.IO.File.Create(path);
                await ImageFile.CopyToAsync(stream);

                Vehicle.ImageUrl = "/uploads/" + fileName;
            }

            await _vehicleRepo.AddAsync(Vehicle); 
            return RedirectToPage("Index");
        }
    }
}
