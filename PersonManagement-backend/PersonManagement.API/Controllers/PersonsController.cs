using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonManagement.API.Middleware;
using PersonManagement.Application.DTOs;
using PersonManagement.Application.Interfaces;

namespace PersonManagement.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonService personService, ILogger<PersonsController> logger)
        {
            _personService = personService ?? throw new ArgumentNullException(nameof(personService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all persons
        /// </summary>
        /// <returns>List of all persons</returns>
        /// <response code="200">Returns the list of persons</response>
        /// <response code="500">If an internal error occurs</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersons()
        {
            _logger.LogInformation("Getting all persons");

            var persons = await _personService.GetAllPersonsAsync();

            _logger.LogInformation("Retrieved {Count} persons", persons.Count());
            return Ok(persons);
        }

        /// <summary>
        /// Get a specific person by ID
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <returns>The requested person</returns>
        /// <response code="200">Returns the requested person</response>
        /// <response code="404">If the person is not found</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonDto>> GetPerson(int id)
        {
            _logger.LogInformation("Getting person with ID: {PersonId}", id);

            try
            {
                var person = await _personService.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Person with ID {PersonId} not found", id);
                throw new NotFoundException($"Person with ID {id} not found");
            }
        }

        /// <summary>
        /// Create a new person
        /// </summary>
        /// <param name="createPersonDto">Person data</param>
        /// <returns>The created person</returns>
        /// <response code="201">Returns the created person</response>
        /// <response code="400">If the request is invalid</response>
        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonDto>> CreatePerson([FromBody] CreatePersonDto createPersonDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreatePerson");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new person: {FirstName} {LastName}",
                createPersonDto.FirstName, createPersonDto.LastName);

            var person = await _personService.CreatePersonAsync(createPersonDto);

            _logger.LogInformation("Created person with ID: {PersonId}", person.PersonId);

            return CreatedAtAction(
                nameof(GetPerson),
                new { id = person.PersonId, version = "1.0" },
                person);
        }

        /// <summary>
        /// Update an existing person
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <param name="updatePersonDto">Updated person data</param>
        /// <returns>The updated person</returns>
        /// <response code="200">Returns the updated person</response>
        /// <response code="404">If the person is not found</response>
        /// <response code="400">If the request is invalid</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonDto>> UpdatePerson(int id, [FromBody] UpdatePersonDto updatePersonDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdatePerson");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating person with ID: {PersonId}", id);

            try
            {
                var person = await _personService.UpdatePersonAsync(id, updatePersonDto);

                _logger.LogInformation("Updated person with ID: {PersonId}", id);
                return Ok(person);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Person with ID {PersonId} not found for update", id);
                throw new NotFoundException($"Person with ID {id} not found");
            }
        }

        /// <summary>
        /// Delete a person
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <returns>No content</returns>
        /// <response code="204">If the person was deleted successfully</response>
        /// <response code="404">If the person is not found</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePerson(int id)
        {
            _logger.LogInformation("Deleting person with ID: {PersonId}", id);

            try
            {
                await _personService.DeletePersonAsync(id);

                _logger.LogInformation("Deleted person with ID: {PersonId}", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Person with ID {PersonId} not found for deletion", id);
                throw new NotFoundException($"Person with ID {id} not found");
            }
        }

        /// <summary>
        /// Check if a person exists
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <returns>True if exists, false otherwise</returns>
        [HttpHead("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PersonExists(int id)
        {
            try
            {
                await _personService.GetPersonByIdAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}