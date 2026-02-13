namespace IncidentServiceAPI.Models.Entities
{
    public class Account
    {
        public string Name { get; set; }

        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}