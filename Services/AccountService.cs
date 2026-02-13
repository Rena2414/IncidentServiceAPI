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
            if (request == null) throw new ArgumentNullException(nameof(request));

            var accountExists = await _unitOfWork.Accounts
                .ExistsAsync(a => a.Name == request.AccountName);

            if (accountExists)
            {
                throw new ArgumentException($"Account '{request.AccountName}' already exists.");
            }

            var response = new CreateAccountResponseDto();

            await _unitOfWork.ExecuteInTransactionAsync(async cancellationToken =>
            {
                var account = new Account
                {
                    Name = request.AccountName
                };

                await _unitOfWork.Accounts.AddAsync(account, cancellationToken);

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
                    contact.Account = account;

                    _unitOfWork.Contacts.Update(contact);
                }

                response.AccountName = account.Name;
            });

            return response;
        }
    }
}