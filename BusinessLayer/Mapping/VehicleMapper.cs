using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Mapping
{
    public class VehicleMapper
    {
        public static VehicleDto ToVehicleDto(Vehicle vehicle)
        {
            var response = new VehicleDto
            {
                stationId = vehicle.StationId,
                name = vehicle.Name,
                brand = vehicle.Brand,
                plateNumber = vehicle.PlateNumber,
                model = vehicle.Model,
                vehicleType = vehicle.VehicleType,
                status = vehicle.Status,
                pricePerHour = vehicle.PricePerHour,
                pricePerDay = vehicle.PricePerDay,
                features = vehicle.Features,
                imageUrl = vehicle.ImageUrl,
                maxDistance = vehicle.MaxDistance
            };
            return response;
        }

        public static Vehicle ToVehicleEntity(VehicleCreateDto dto)
        {
            var response = new Vehicle
            {
                StationId = dto.stationId,
                Name = dto.name,
                Brand = dto.brand,
                PlateNumber = dto.plateNumber,
                Model = dto.model,
                VehicleType = dto.vehicleType,
                PricePerHour = dto.pricePerHour,
                PricePerDay = dto.pricePerDay,
                Status = DataAccessLayer.Enums.VehicleStatus.Available,
                Features = dto.features,
                ImageUrl = dto.imageUrl,
                MaxDistance = dto.maxDistance
            };
            return response;
        }

        public static void UpdateVehicleEntity(Vehicle vehicle, VehicleUpdateDto dto)
        {
            vehicle.StationId = dto.stationId;
            vehicle.Name = dto.name;
            vehicle.Brand = dto.brand;
            vehicle.PlateNumber = dto.plateNumber;
            vehicle.Model = dto.model;
            vehicle.VehicleType = dto.vehicleType;
            vehicle.Status = dto.status;
            vehicle.PricePerHour = dto.pricePerHour;
            vehicle.PricePerDay = dto.pricePerDay;
            vehicle.Features = dto.features;
            vehicle.ImageUrl = dto.imageUrl;
            vehicle.MaxDistance = dto.maxDistance;
        }

    }
}
