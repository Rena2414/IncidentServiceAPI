using Microsoft.EntityFrameworkCore;
using IncidentServiceAPI.Services.Interfaces;
using IncidentServiceAPI.Data;
using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Models.Entities;

namespace IncidentServiceAPI.Services
{
    /// <summary>
    /// Implements incident creation logic.
    /// Ensures account existence, contact upsert, relationship linking, and incident persistence.
    /// </summary>
    public class IncidentService : IIncidentService
    {
        private readonly AppDbContext _context;

        public IncidentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateIncidentResponseDto> CreateIncidentAsync(CreateIncidentRequestDto request)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Name == request.AccountName);

            if (account == null)
            {
                throw new KeyNotFoundException($"Account '{request.AccountName}' not found.");
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

            var isLinked = await _context.AccountContacts.AnyAsync(ac =>
                ac.AccountName == account.Name &&
                ac.ContactEmail == contact.Email);

            if (!isLinked)
            {
                var accountContact = new AccountContact
                {
                    AccountName = account.Name,
                    ContactEmail = contact.Email
                };

                _context.AccountContacts.Add(accountContact);
            }

            var incident = new Incident
            {
                IncidentName = Guid.NewGuid().ToString("N"),
                Description = request.IncidentDescription,
                AccountName = account.Name
            };

            _context.Incidents.Add(incident);

            await _context.SaveChangesAsync();

            return new CreateIncidentResponseDto
            {
                IncidentName = incident.IncidentName,
                AccountName = account.Name
            };
        }
    }
}