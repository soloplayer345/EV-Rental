using BusinessLayer.DTOs;
using DataAccessLayer.Entities;

namespace BusinessLayer.Mapping
{
    public static class InspectionProblemMapper
    {
        public static InspectionProblemDto ToDto(this InspectionProblem problem)
        {
            if (problem == null) return null;

            return new InspectionProblemDto
            {
                Id = problem.Id,
                RentalId = problem.RentalId,
                IncidentType = problem.IncidentType,
                Description = problem.Description,
                Evidence = problem.Evidence,
                ImageUrl = problem.Evidence,
                PenaltyAmount = problem.PenaltyAmount,
                CreatedBy = problem.CreatedBy,
                CreateDate = problem.CreateDate
            };
        }

        public static List<InspectionProblemDto> ToDtoList(IEnumerable<InspectionProblem> problems)
        {
            return problems?.Select(p => p.ToDto()).ToList() ?? new List<InspectionProblemDto>();
        }

        public static InspectionProblem ToEntity(this InspectionProblemDto dto)
        {
            if (dto == null) return null;

            return new InspectionProblem
            {
                Id = dto.Id,
                RentalId = dto.RentalId,
                IncidentType = dto.IncidentType,
                Description = dto.Description,
                Evidence = dto.Evidence ?? string.Empty,
                PenaltyAmount = dto.PenaltyAmount,
                CreatedBy = dto.CreatedBy,
                CreateDate = dto.CreateDate,
                UpdateDate = DateTime.Now
            };
        }
    }
}
