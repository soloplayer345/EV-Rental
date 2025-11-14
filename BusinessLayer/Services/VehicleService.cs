using BusinessLayer.DTOs;
using BusinessLayer.Mapping;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class VehicleService
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public VehicleService(IVehicleRepo vehicleRepo, IUnitOfWork unitOfWork)
        {
            _vehicleRepo = vehicleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VehicleDto>> SearchVehiclesAsync(string name, string brand, DataAccessLayer.Enums.VehicleStatus status)
        {
            var vehicles = await _vehicleRepo.SearchVehiclesAsync(name, brand, status);
            return vehicles.Select(VehicleMapper.ToVehicleDto);
        }

        public async Task<IEnumerable<VehicleDto>> GetVehiclesAsync()
        {
            var vehicles = await _vehicleRepo.GetAllAsync();
            return vehicles.Select(VehicleMapper.ToVehicleDto);
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepo.GetAllAsync();
            return vehicles.Select(VehicleMapper.ToVehicleDto);
        }

        public async Task<IEnumerable<VehicleDto>> GetVehiclesByStationIdAsync(int stationId)
        {
            var allVehicles = await _vehicleRepo.GetAllAsync();
            var filtered = allVehicles.Where(v => v.StationId == stationId);
            return filtered.Select(VehicleMapper.ToVehicleDto);
        }

        public async Task<VehicleDto> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            return vehicle != null ? VehicleMapper.ToVehicleDto(vehicle) : null;
        }

        public async Task AddVehicleAsync(VehicleCreateDto vehicleDto)
        {
            var vehicle = VehicleMapper.ToVehicleEntity(vehicleDto);
            await _vehicleRepo.AddAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateVehicleAsync(VehicleUpdateDto vehicleDto)
        {
            var existingVehicle = await _vehicleRepo.GetByIdAsync(vehicleDto.Id);
            if (existingVehicle != null)
            {
                VehicleMapper.UpdateVehicleEntity(existingVehicle, vehicleDto);
                await _vehicleRepo.Update(existingVehicle);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdateVehicleAsync(VehicleDto vehicleDto)
        {
            var existingVehicle = await _vehicleRepo.GetByIdAsync(vehicleDto.Id);
            if (existingVehicle != null)
            {
                // Map VehicleDto to entity
                existingVehicle.StationId = vehicleDto.StationId;
                existingVehicle.Name = vehicleDto.Name;
                existingVehicle.Brand = vehicleDto.Brand;
                existingVehicle.PlateNumber = vehicleDto.PlateNumber;
                existingVehicle.Model = vehicleDto.Model;
                existingVehicle.VehicleType = vehicleDto.VehicleType;
                existingVehicle.Status = vehicleDto.Status;
                existingVehicle.PricePerHour = vehicleDto.PricePerHour;
                existingVehicle.PricePerDay = vehicleDto.PricePerDay;
                existingVehicle.Features = vehicleDto.Features;
                existingVehicle.ImageUrl = vehicleDto.ImageUrl;
                existingVehicle.MaxDistance = vehicleDto.MaxDistance;
                existingVehicle.BatteryCapacity = vehicleDto.BatteryCapacity;

                await _vehicleRepo.Update(existingVehicle);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (vehicle != null)
            {
                // Check for rental records related to this vehicle
                var rentalRecordRepo = _unitOfWork.GetRepository<RentalRecord>();
                var paymentRepo = _unitOfWork.GetRepository<Payment>();
                var inspectionProblemRepo = _unitOfWork.GetRepository<InspectionProblem>();
                var ratingReviewRepo = _unitOfWork.GetRepository<RatingReview>();

                var relatedRentalRecords = (await rentalRecordRepo.GetAllAsync())
                    .Where(r => r.VehicleId == id)
                    .ToList();

                // Delete all related data in cascade order
                if (relatedRentalRecords.Count > 0)
                {
                    foreach (var rentalRecord in relatedRentalRecords)
                    {
                        // Delete Payments
                        var payments = (await paymentRepo.GetAllAsync())
                            .Where(p => p.RentalId == rentalRecord.Id)
                            .ToList();
                        foreach (var payment in payments)
                        {
                            await paymentRepo.Delete(payment);
                        }

                        // Delete InspectionProblems
                        var inspectionProblems = (await inspectionProblemRepo.GetAllAsync())
                            .Where(i => i.RentalId == rentalRecord.Id)
                            .ToList();
                        foreach (var inspectionProblem in inspectionProblems)
                        {
                            await inspectionProblemRepo.Delete(inspectionProblem);
                        }

                        // Delete RatingReview
                        var ratingReviews = (await ratingReviewRepo.GetAllAsync())
                            .Where(r => r.RentalId == rentalRecord.Id)
                            .ToList();
                        foreach (var ratingReview in ratingReviews)
                        {
                            await ratingReviewRepo.Delete(ratingReview);
                        }

                        // Delete RentalRecord
                        await rentalRecordRepo.Delete(rentalRecord);
                    }
                }

                await _vehicleRepo.Delete(vehicle);
                await _unitOfWork.SaveChangesAsync();
            }
        }

    }
}


