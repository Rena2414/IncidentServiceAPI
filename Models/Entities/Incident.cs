namespace IncidentServiceAPI.Models.Entities
{
    public class Incident
    {
        public string IncidentName { get; set; }

        public string Description { get; set; }

        public string AccountName { get; set; }
        public Account Account { get; set; }
    }
}