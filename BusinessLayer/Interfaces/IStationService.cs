using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IStationService
    {
        Task<IEnumerable<StationDto>> GetAllStationsAsync();
        Task<StationDto> GetStationByIdAsync(int id);
        Task AddStationAsync(StationDto stationDto);
        Task UpdateStationAsync(StationDto stationDto);
        Task DeleteStationAsync(int id);
        Task<IEnumerable<StationDto>> SearchStationsAsync(string name, string state);
    }
}
