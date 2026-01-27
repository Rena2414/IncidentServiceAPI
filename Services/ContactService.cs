using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Models.Entities;
using IncidentServiceAPI.Repositories.Interfaces;
using IncidentServiceAPI.Services.Interfaces;

namespace IncidentServiceAPI.Services
{
    /// <summary>
    /// Implements contact-related business logic.
    /// Creates or updates a contact identified by email.
    /// </summary>
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateContactResponseDto> CreateOrUpdateContactAsync(CreateContactRequestDto request)
        {
            CreateContactResponseDto response = null!;
            await _unitOfWork.ExecuteInTransactionAsync(async cancellationToken =>
            {
                var contact = await _unitOfWork.Contacts
                    .GetAsync(c => c.Email == request.ContactEmail, cancellationToken);

                var isCreated = false;

                if (contact == null)
                {
                    contact = new Contact
                    {
                        Email = request.ContactEmail,
                        FirstName = request.ContactFirstName,
                        LastName = request.ContactLastName
                    };

                    await _unitOfWork.Contacts.AddAsync(contact, cancellationToken);
                    isCreated = true;
                }
                else
                {
                    contact.FirstName = request.ContactFirstName;
                    contact.LastName = request.ContactLastName;
                    _unitOfWork.Contacts.Update(contact);
                }

                response = new CreateContactResponseDto
                {
                    Email = contact.Email,
                    IsCreated = isCreated
                };
            });

            return response;
        }
    }
}