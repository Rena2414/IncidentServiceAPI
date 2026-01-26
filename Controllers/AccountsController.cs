using Microsoft.AspNetCore.Mvc;
using IncidentServiceAPI.Models.DTOs;
using IncidentServiceAPI.Services.Interfaces;

namespace IncidentServiceAPI.Controllers
{
    /// <summary>
    /// Accounts API endpoints.
    /// Keeps controllers thin: validation + HTTP semantics, business logic in services.
    /// </summary>
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Creates a new account and associates it with a contact (creates or updates contact details).
        /// Returns 201 Created on success.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAccount(
            [FromBody] CreateAccountRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _accountService.CreateAccountAsync(request);

            return CreatedAtAction(
                nameof(CreateAccount),
                new { name = result.AccountName },
                result);
        }
    }
}