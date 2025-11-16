using BusinessLayer.DTOs;
using DataAccessLayer.Entities;

namespace BusinessLayer.Mapping
{
    public static class StationMapper
    {
        public static StationDto ToDto(this Station station)
        {
            if (station == null) return null;

            return new StationDto
            {
                Id = station.Id,
                Name = station.Name,
                Address = station.Address,
                State = station.State
            };
        }

        public static Station ToEntity(StationDto dto)
        {
            if (dto == null) return null;

            return new Station
            {
                Id = dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                State = dto.State
            };
        }

        public static List<StationDto> ToDtoList(IEnumerable<Station> stations)
        {
            return stations?.Select(ToDto).ToList() ?? new List<StationDto>();
        }
    }
}
