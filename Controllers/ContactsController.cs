using Microsoft.AspNetCore.Mvc;
using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Services.Interfaces;

namespace IncidentServiceAPI.Controllers
{
    /// <summary>
    /// Contacts API endpoints.
    /// </summary>
    [ApiController]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Creates or updates a contact (identified by email).
        /// Returns 201 if created, 200 if updated.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateContact(
            [FromBody] CreateContactRequestDto request)
        {
            var result = await _contactService.CreateOrUpdateContactAsync(request);

            if (result.IsCreated)
            {
                return StatusCode(201, result);
            }

            return Ok(result);
        }
    }
}