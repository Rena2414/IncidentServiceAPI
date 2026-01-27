using IncidentServiceAPI.Data;
using IncidentServiceAPI.Repositories.Interfaces;

namespace IncidentServiceAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Accounts = new AccountRepository(context);
            Contacts = new ContactRepository(context);
            AccountContacts = new AccountContactRepository(context);
            Incidents = new IncidentRepository(context);
        }

        public IAccountRepository Accounts { get; }
        public IContactRepository Contacts { get; }
        public IAccountContactRepository AccountContacts { get; }
        public IIncidentRepository Incidents { get; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}