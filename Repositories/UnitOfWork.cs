using IncidentServiceAPI.Data;
using IncidentServiceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace IncidentServiceAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(
            AppDbContext context,
            IAccountRepository accounts,
            IContactRepository contacts,
            IIncidentRepository incidents)
        {
            _context = context;
            Accounts = accounts;
            Contacts = contacts;
            Incidents = incidents;
        }

        public IAccountRepository Accounts { get; }
        public IContactRepository Contacts { get; }
        public IIncidentRepository Incidents { get; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            await using IDbContextTransaction transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            await action(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}