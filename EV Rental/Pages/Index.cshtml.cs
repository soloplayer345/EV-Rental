using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace EV_Rental.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly CarAiAssistantService _carAiAssistant;

        public IndexModel(IVehicleService vehicleService, CarAiAssistantService carAiAssistant)
        {
            _vehicleService = vehicleService;
            _carAiAssistant = carAiAssistant;
        }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Brand { get; set; }

        [BindProperty(SupportsGet = true)]
        public VehicleStatus? Status { get; set; }

        [BindProperty]
        public string UserQuery { get; set; }

        public IEnumerable<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();

        public List<AiResultDto> AiRecommendedVehicles { get; set; } = new();

        public async Task OnGetAsync()
        {
            Vehicles = Status.HasValue
                ? await _vehicleService.SearchVehiclesAsync(Name ?? string.Empty, Brand ?? string.Empty, Status.Value)
                : await _vehicleService.GetVehiclesAsync();
        }

        public async Task<IActionResult> OnPostChatAsync()
        {
            Vehicles = await _vehicleService.GetVehiclesAsync();
            var availableVehicles = Vehicles.Where(v => v.Status == VehicleStatus.Available).ToList();

            AiRecommendedVehicles.Clear();

            if (!string.IsNullOrWhiteSpace(UserQuery))
            {
                var aiJson = await _carAiAssistant.AskAsync(UserQuery, availableVehicles);

                try
                {
                    AiRecommendedVehicles.Clear();

                    using var doc = JsonDocument.Parse(aiJson);

                    // 1. Summary
                    if (doc.RootElement.TryGetProperty("Summary", out var summaryProp))
                    {
                        string summaryText = summaryProp.GetString() ?? "";
                        if (!string.IsNullOrWhiteSpace(summaryText))
                        {
                            AiRecommendedVehicles.Add(new AiResultDto
                            {
                                Id = null,  // summary
                                Name = "",
                                Reason = summaryText
                            });
                        }
                    }

                    // 2. Cars array
                    if (doc.RootElement.TryGetProperty("Cars", out var carsProp) && carsProp.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var carEl in carsProp.EnumerateArray())
                        {
                            var carDto = new AiResultDto
                            {
                                Id = carEl.GetProperty("Id").GetInt32(),
                                Name = carEl.GetProperty("Name").GetString() ?? "",
                                Reason = carEl.GetProperty("Reason").GetString() ?? ""
                            };
                            AiRecommendedVehicles.Add(carDto);
                        }
                    }
                }
                catch
                {
                    // fallback: treat everything as summary
                    AiRecommendedVehicles.Add(new AiResultDto
                    {
                        Id = null,
                        Name = "",
                        Reason = aiJson
                    });
                }
            }

            return Page();
        }



    }
}
