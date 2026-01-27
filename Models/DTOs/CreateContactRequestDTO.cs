using System.ComponentModel.DataAnnotations;

namespace IncidentServiceAPI.Models.DTOs
{
    public class CreateContactRequestDto
    {
        [Required]
        [StringLength(100)]
        public string contactFirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string contactLastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string contactEmail { get; set; }
    }
}