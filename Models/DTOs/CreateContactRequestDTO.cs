using System.ComponentModel.DataAnnotations;

namespace IncidentServiceAPI.Models.DTOs
{
    public class CreateContactRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
    }
}