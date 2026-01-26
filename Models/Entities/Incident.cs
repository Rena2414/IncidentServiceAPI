namespace IncidentServiceAPI.Models.Entities
{
    public class Incident
    {
        public required string IncidentName { get; set; }

        public required string Description { get; set; }

        public required string AccountName { get; set; }
        public required Account Account { get; set; }
    }
}