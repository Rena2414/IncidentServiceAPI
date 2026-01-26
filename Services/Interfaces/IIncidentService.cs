using IncidentServiceAPI.Models.DTOs;

namespace IncidentServiceAPI.Services.Interfaces
{
    public interface IIncidentService
    {
        Task<CreateIncidentResponseDto> CreateIncidentAsync(CreateIncidentRequestDto request);
    }
}
