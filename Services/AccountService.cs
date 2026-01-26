using Microsoft.EntityFrameworkCore;
using IncidentServiceAPI.Services.Interfaces;
using IncidentServiceAPI.Data;
using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Models.Entities;

namespace IncidentServiceAPI.Services
{
    /// <summary>
    /// Implements account-related business logic.
    /// Handles uniqueness, contact upsert, and relationship creation.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateAccountResponseDto> CreateAccountAsync(CreateAccountRequestDto request)
        {
            var accountExists = await _context.Accounts
                .AnyAsync(a => a.Name == request.AccountName);

            if (accountExists)
            {
                throw new ArgumentException($"Account '{request.AccountName}' already exists.");
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Email == request.ContactEmail);

            if (contact == null)
            {
                contact = new Contact
                {
                    Email = request.ContactEmail,
                    FirstName = request.ContactFirstName,
                    LastName = request.ContactLastName
                };

                _context.Contacts.Add(contact);
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

            _context.Accounts.Add(account);

            var accountContact = new AccountContact
            {
                AccountName = account.Name,
                ContactEmail = contact.Email
            };

            _context.AccountContacts.Add(accountContact);

            await _context.SaveChangesAsync();

            return new CreateAccountResponseDto
            {
                AccountName = account.Name
            };
        }
    }
}