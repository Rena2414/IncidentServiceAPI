using System.ComponentModel.DataAnnotations;

namespace IncidentServiceAPI.Models.DTOs
{
    public class CreateAccountRequestDto
    {
        [Required]
        [StringLength(255)]
        public string AccountName { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactFirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactLastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string ContactEmail { get; set; }
    }
}