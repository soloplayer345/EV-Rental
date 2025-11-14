using BusinessLayer.DTOs;
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

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string name, string brand, DataAccessLayer.Enums.VehicleStatus status)
        {
            return await _vehicleRepo.SearchVehiclesAsync(name, brand, status);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync()
        {
            return await _vehicleRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehicleRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByStationIdAsync(int stationId)
        {
            var allVehicles = await _vehicleRepo.GetAllAsync();
            return allVehicles.Where(v => v.StationId == stationId);
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _vehicleRepo.GetByIdAsync(id);
        }

        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await _vehicleRepo.AddAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            await _vehicleRepo.Update(vehicle);
            await _unitOfWork.SaveChangesAsync();
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

