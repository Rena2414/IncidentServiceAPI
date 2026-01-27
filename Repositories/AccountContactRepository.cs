using IncidentServiceAPI.Data;
using IncidentServiceAPI.Models.Entities;
using IncidentServiceAPI.Repositories.Interfaces;

namespace IncidentServiceAPI.Repositories
{
    public class AccountContactRepository : Repository<AccountContact>, IAccountContactRepository
    {
        public AccountContactRepository(AppDbContext context) : base(context) { }
    }
}