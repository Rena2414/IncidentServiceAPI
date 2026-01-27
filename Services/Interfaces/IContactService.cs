using IncidentServiceAPI.Models.DTOs;

namespace IncidentServiceAPI.Services.Interfaces
{
    public interface IContactService
    {
        Task<CreateContactResponseDto> CreateOrUpdateContactAsync(CreateContactRequestDto request);
    }
}