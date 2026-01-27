using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Models.Entities;
using IncidentServiceAPI.Repositories.Interfaces;
using IncidentServiceAPI.Services.Interfaces;

namespace IncidentServiceAPI.Services
{
    /// <summary>
    /// Implements account-related business logic.
    /// Handles uniqueness, contact upsert, and relationship creation.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateAccountResponseDto> CreateAccountAsync(CreateAccountRequestDto request)
        {
            var accountExists = await _unitOfWork.Accounts
                .ExistsAsync(a => a.Name == request.AccountName);

            if (accountExists)
            {
                throw new ArgumentException($"Account '{request.AccountName}' already exists.");
            }

            var contact = await _unitOfWork.Contacts
                .GetAsync(c => c.Email == request.ContactEmail);

            if (contact == null)
            {
                contact = new Contact
                {
                    Email = request.ContactEmail,
                    FirstName = request.ContactFirstName,
                    LastName = request.ContactLastName
                };

                await _unitOfWork.Contacts.AddAsync(contact);
            }
            else
            {
                contact.FirstName = request.ContactFirstName;
                contact.LastName = request.ContactLastName;
            }

            var account = new Account
            {
                Name = request.AccountName
            };

            await _unitOfWork.Accounts.AddAsync(account);

            var accountContact = new AccountContact
            {
                AccountName = account.Name,
                ContactEmail = contact.Email
            };

            await _unitOfWork.AccountContacts.AddAsync(accountContact);

            await _unitOfWork.SaveChangesAsync();

            return new CreateAccountResponseDto
            {
                AccountName = account.Name
            };
        }
    }
}