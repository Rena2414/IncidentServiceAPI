namespace IncidentServiceAPI.Models.Entities
{
    public class Contact
    {
        public required string Email { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public ICollection<AccountContact> AccountContacts { get; set; } = new List<AccountContact>();
    }
}