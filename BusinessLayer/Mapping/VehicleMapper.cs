using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Mapping
{
    public static class VehicleMapper
    {
        public static VehicleDto ToVehicleDto(this Vehicle vehicle)
        {
            if (vehicle == null) return null;

            var response = new VehicleDto
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
                Station = vehicle.Station != null ? vehicle.Station.ToDto() : null
            };
            return response;
        }

        public static Vehicle ToVehicleEntity(VehicleCreateDto dto)
        {
            var response = new Vehicle
            {
                StationId = dto.StationId,
                Name = dto.Name,
                Brand = dto.Brand,
                PlateNumber = dto.PlateNumber,
                Model = dto.Model,
                VehicleType = dto.VehicleType,
                PricePerHour = dto.PricePerHour,
                PricePerDay = dto.PricePerDay,
                Status = dto.Status,
                Features = dto.Features,
                ImageUrl = dto.ImageUrl,
                MaxDistance = dto.MaxDistance,
                BatteryCapacity = dto.BatteryCapacity,
                seartCapacity = dto.seartCapacity
            };
            return response;
        }

        public static void UpdateVehicleEntity(Vehicle vehicle, VehicleUpdateDto dto)
        {
            vehicle.StationId = dto.StationId;
            vehicle.Name = dto.Name;
            vehicle.Brand = dto.Brand;
            vehicle.PlateNumber = dto.PlateNumber;
            vehicle.Model = dto.Model;
            vehicle.VehicleType = dto.VehicleType;
            vehicle.Status = dto.Status;
            vehicle.PricePerHour = dto.PricePerHour;
            vehicle.PricePerDay = dto.PricePerDay;
            vehicle.Features = dto.Features;
            vehicle.ImageUrl = dto.ImageUrl;
            vehicle.MaxDistance = dto.MaxDistance;
            vehicle.BatteryCapacity = dto.BatteryCapacity;
            vehicle.seartCapacity = dto.seartCapacity;
        }

    }
}
