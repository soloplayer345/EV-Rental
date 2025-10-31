using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class DeleteVehicleModel : PageModel
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVehicleModel(IVehicleRepo vehicleRepo, IUnitOfWork unitOfWork)
        {
            _vehicleRepo = vehicleRepo;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (Vehicle == null) return RedirectToPage("Index");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _vehicleRepo.Delete(Vehicle);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
