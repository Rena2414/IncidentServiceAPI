using IncidentServiceAPI.Models.DTOs;

namespace IncidentServiceAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Task<CreateAccountResponseDto> CreateAccountAsync(CreateAccountRequestDto request);
    }
}
