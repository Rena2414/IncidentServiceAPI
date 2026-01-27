using IncidentServiceAPI.Data;
using IncidentServiceAPI.Models.Entities;
using IncidentServiceAPI.Repositories.Interfaces;

namespace IncidentServiceAPI.Repositories
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        public ContactRepository(AppDbContext context) : base(context) { }
    }
}