using BusinessLayer.DTOs;
using BusinessLayer.Mapping;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class StationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StationDto>> GetAllStationsAsync()
        {
            var stationRepo = _unitOfWork.GetRepository<Station>();
            var stations = stationRepo.GetAllQueryable("Vehicles");
            var stationList = await stations.ToListAsync();
            return StationMapper.ToDtoList(stationList);
        }

        public async Task<StationDto> GetStationByIdAsync(int id)
        {
            var stationRepo = _unitOfWork.GetRepository<Station>();
            var station = await stationRepo.FindOneAsync(s => s.Id == id, "Vehicles");
            return StationMapper.ToDto(station);
        }

        public async Task AddStationAsync(StationDto stationDto)
        {
            var stationRepo = _unitOfWork.GetRepository<Station>();
            var station = StationMapper.ToEntity(stationDto);
            await stationRepo.AddAsync(station);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStationAsync(StationDto stationDto)
        {
            var stationRepo = _unitOfWork.GetRepository<Station>();
            var station = StationMapper.ToEntity(stationDto);
            stationRepo.Update(station);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteStationAsync(int id)
        {
            var stationRepo = _unitOfWork.GetRepository<Station>();
            var station = await stationRepo.GetByIdAsync(id);
            if (station != null)
            {
                stationRepo.Delete(station);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StationDto>> SearchStationsAsync(string name, string state)
        {
            var stationRepo = _unitOfWork.GetRepository<Station>();
            var stations = await stationRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(name))
            {
                stations = stations.Where(s =>
                    s.Name.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                    s.Address.Contains(name, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(state))
            {
                stations = stations.Where(s =>
                    s.State.Equals(state, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return StationMapper.ToDtoList(stations);
        }
    }
}
