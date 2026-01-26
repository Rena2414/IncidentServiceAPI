namespace IncidentServiceAPI.Models.Entities
{
    public class AccountContact
    {
        public string AccountName { get; set; }
        public Account Account { get; set; }

        public string ContactEmail { get; set; }
        public Contact Contact { get; set; }
    }
}