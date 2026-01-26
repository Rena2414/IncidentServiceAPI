using Microsoft.AspNetCore.Mvc;
using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Services.Interfaces;

namespace IncidentServiceAPI.Controllers
{
    /// <summary>
    /// Incidents API endpoints.
    /// </summary>
    [ApiController]
    [Route("api/incidents")]
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentsController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        /// <summary>
        /// Creates an incident for an existing account.
        /// Also ensures the provided contact exists and is linked to the account.
        /// Returns 201 Created on success.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateIncident(
            [FromBody] CreateIncidentRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _incidentService.CreateIncidentAsync(request);

            return CreatedAtAction(
                nameof(CreateIncident),
                new { name = result.IncidentName },
                result);
        }
    }
}