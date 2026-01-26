namespace IncidentServiceAPI.Models.Entities
{
    public class Contact
    {
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<AccountContact> AccountContacts { get; set; } = new List<AccountContact>();
    }
}