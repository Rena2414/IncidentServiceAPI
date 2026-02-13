namespace IncidentServiceAPI.Models.Entities
{
    public class Contact
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? AccountName { get; set; }
        public Account? Account { get; set; }
    }
}