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
            var contact = await _unitOfWork.Contacts
                .GetAsync(c => c.Email == request.Email);

            var isCreated = false;

            if (contact == null)
            {
                contact = new Contact
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                await _unitOfWork.Contacts.AddAsync(contact);
                isCreated = true;
            }
            else
            {
                contact.FirstName = request.FirstName;
                contact.LastName = request.LastName;
            }

            await _unitOfWork.SaveChangesAsync();

            return new CreateContactResponseDto
            {
                Email = contact.Email,
                IsCreated = isCreated
            };
        }
    }
}