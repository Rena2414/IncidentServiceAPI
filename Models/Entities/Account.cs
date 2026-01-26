namespace IncidentServiceAPI.Models.Entities
{
    public class Account
    {
        public string Name { get; set; }

        public ICollection<AccountContact> AccountContacts { get; set; } = new List<AccountContact>();
        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}