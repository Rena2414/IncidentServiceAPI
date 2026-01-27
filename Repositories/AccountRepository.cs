using IncidentServiceAPI.Data;
using IncidentServiceAPI.Models.Entities;
using IncidentServiceAPI.Repositories.Interfaces;

namespace IncidentServiceAPI.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context) { }
    }
}