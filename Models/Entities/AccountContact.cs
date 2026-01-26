namespace IncidentServiceAPI.Models.Entities
{
    public class AccountContact
    {
        public required string AccountName { get; set; }
        public required Account Account { get; set; }

        public required string ContactEmail { get; set; }
        public required Contact Contact { get; set; }
    }
}