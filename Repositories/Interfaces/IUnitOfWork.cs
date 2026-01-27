namespace IncidentServiceAPI.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        IContactRepository Contacts { get; }
        IAccountContactRepository AccountContacts { get; }
        IIncidentRepository Incidents { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);
    }
}