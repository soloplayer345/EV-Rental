using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using System.Collections.Generic;
using System.Linq;

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
                State = station.State,
                Vehicles = station.Vehicles?.Select(v => ToVehicleDtoWithoutStation(v)).ToList() ?? new List<VehicleDto>()
            };
        }
        
        private static VehicleDto ToVehicleDtoWithoutStation(Vehicle vehicle)
        {
            if (vehicle == null) return null;

            return new VehicleDto
            {
                Id = vehicle.Id,
                StationId = vehicle.StationId,
                Name = vehicle.Name,
                Brand = vehicle.Brand,
                PlateNumber = vehicle.PlateNumber,
                Model = vehicle.Model,
                VehicleType = vehicle.VehicleType,
                Status = vehicle.Status,
                PricePerHour = vehicle.PricePerHour,
                PricePerDay = vehicle.PricePerDay,
                Features = vehicle.Features,
                ImageUrl = vehicle.ImageUrl,
                MaxDistance = vehicle.MaxDistance,
                BatteryCapacity = vehicle.BatteryCapacity,
                seartCapacity = vehicle.seartCapacity,
                Station = null // Don't map station to avoid circular reference
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
