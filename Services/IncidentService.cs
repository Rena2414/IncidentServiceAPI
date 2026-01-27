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
            Incident incident = null!;
            string accountName = null!;

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
                        LastName = request.ContactLastName
                    };

                    await _unitOfWork.Contacts.AddAsync(contact, cancellationToken);
                }
                else
                {
                    contact.FirstName = request.ContactFirstName;
                    contact.LastName = request.ContactLastName;
                }

                var isLinked = await _unitOfWork.AccountContacts.ExistsAsync(ac =>
                    ac.AccountName == account.Name &&
                    ac.ContactEmail == contact.Email, cancellationToken);

                if (!isLinked)
                {
                    var accountContact = new AccountContact
                    {
                        AccountName = account.Name,
                        ContactEmail = contact.Email
                    };

                    await _unitOfWork.AccountContacts.AddAsync(accountContact, cancellationToken);
                }

                incident = new Incident
                {
                    Description = request.IncidentDescription,
                    AccountName = account.Name
                };

                await _unitOfWork.Incidents.AddAsync(incident, cancellationToken);
                accountName = account.Name;
            });

            return new CreateIncidentResponseDto
            {
                IncidentName = incident.IncidentName,
                AccountName = accountName
            };
        }
    }
}