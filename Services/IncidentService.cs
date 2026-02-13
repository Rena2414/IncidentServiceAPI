using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Models.Entities;
using IncidentServiceAPI.Repositories.Interfaces;
using IncidentServiceAPI.Services.Interfaces;

namespace IncidentServiceAPI.Services
{
    /// <summary>
    /// Implements incident creation logic.
    /// Ensures account existence, contact upsert, relationship linking, and incident persistence.
    /// </summary>
    public class IncidentService : IIncidentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncidentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateIncidentResponseDto> CreateIncidentAsync(CreateIncidentRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var response = new CreateIncidentResponseDto();

            await _unitOfWork.ExecuteInTransactionAsync(async cancellationToken =>
            {
                var account = await _unitOfWork.Accounts
                    .GetAsync(a => a.Name == request.AccountName, cancellationToken);

                if (account == null)
                {
                    throw new KeyNotFoundException($"Account '{request.AccountName}' not found.");
                }

                var contact = await _unitOfWork.Contacts
                    .GetAsync(c => c.Email == request.ContactEmail, cancellationToken);

                if (contact == null)
                {
                    contact = new Contact
                    {
                        Email = request.ContactEmail,
                        FirstName = request.ContactFirstName,
                        LastName = request.ContactLastName,
                        Account = account
                    };
                    await _unitOfWork.Contacts.AddAsync(contact, cancellationToken);
                }
                else
                {
                    contact.FirstName = request.ContactFirstName;
                    contact.LastName = request.ContactLastName;

                    if (contact.AccountName != account.Name)
                    {
                        contact.Account = account;
                    }

                    _unitOfWork.Contacts.Update(contact);
                }

                var incident = new Incident
                {
                    Description = request.IncidentDescription,
                    Account = account
                };

                await _unitOfWork.Incidents.AddAsync(incident, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                response.IncidentName = incident.IncidentName;
                response.AccountName = account.Name;
            });

            return response;
        }
    }
}