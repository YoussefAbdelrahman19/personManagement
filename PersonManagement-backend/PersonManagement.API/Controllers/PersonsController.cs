using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonManagement.API.Middleware;
using PersonManagement.Application.DTOs;
using PersonManagement.Application.Interfaces;

namespace PersonManagement.API.Controllers { 
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Produces("application/json")]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
//    public class PersonsController : ControllerBase
//    {
//        private readonly IPersonService _personService;
//        private readonly ILogger<PersonsController> _logger;

//        public PersonsController(IPersonService personService, ILogger<PersonsController> logger)
//        {
//            _personService = personService ?? throw new ArgumentNullException(nameof(personService));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        [HttpGet]
//        [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
//        [ResponseCache(Duration = 60)]
//        public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersons()
//        {
//            _logger.LogInformation("Getting all persons");
//            var persons = await _personService.GetAllPersonsAsync();
//            _logger.LogInformation("Retrieved {Count} persons", persons.Count());
//            return Ok(persons);
//        }

//        [HttpGet("{id:int}")]
//        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<ActionResult<PersonDto>> GetPerson(int id)
//        {
//            _logger.LogInformation("Getting person with ID: {PersonId}", id);

//            try
//            {
//                var person = await _personService.GetPersonByIdAsync(id);
//                return Ok(person);
//            }
//            catch (KeyNotFoundException)
//            {
//                _logger.LogWarning("Person with ID {PersonId} not found", id);
//                throw new NotFoundException($"Person with ID {id} not found");
//            }
//        }

//        [HttpPost]
//        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<ActionResult<PersonDto>> CreatePerson([FromBody] CreatePersonDto createPersonDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Invalid model state for CreatePerson");
//                return BadRequest(ModelState);
//            }

//            _logger.LogInformation("Creating new person: {FirstName} {LastName}",
//                createPersonDto.FirstName, createPersonDto.LastName);

//            var person = await _personService.CreatePersonAsync(createPersonDto);
//            _logger.LogInformation("Created person with ID: {PersonId}", person.PersonId);

//            return CreatedAtAction(nameof(GetPerson), new { id = person.PersonId }, person);
//        }

//        [HttpPut("{id:int}")]
//        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<ActionResult<PersonDto>> UpdatePerson(int id, [FromBody] UpdatePersonDto updatePersonDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Invalid model state for UpdatePerson");
//                return BadRequest(ModelState);
//            }

//            _logger.LogInformation("Updating person with ID: {PersonId}", id);

//            try
//            {
//                var person = await _personService.UpdatePersonAsync(id, updatePersonDto);
//                _logger.LogInformation("Updated person with ID: {PersonId}", id);
//                return Ok(person);
//            }
//            catch (KeyNotFoundException)
//            {
//                _logger.LogWarning("Person with ID {PersonId} not found for update", id);
//                throw new NotFoundException($"Person with ID {id} not found");
//            }
//        }

//        [HttpDelete("{id:int}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> DeletePerson(int id)
//        {
//            _logger.LogInformation("Deleting person with ID: {PersonId}", id);

//            try
//            {
//                await _personService.DeletePersonAsync(id);
//                _logger.LogInformation("Deleted person with ID: {PersonId}", id);
//                return NoContent();
//            }
//            catch (KeyNotFoundException)
//            {
//                _logger.LogWarning("Person with ID {PersonId} not found for deletion", id);
//                throw new NotFoundException($"Person with ID {id} not found");
//            }
//        }

//        [HttpHead("{id:int}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> PersonExists(int id)
//        {
//            try
//            {
//                await _personService.GetPersonByIdAsync(id);
//                return Ok();
//            }
//            catch (KeyNotFoundException)
//            {
//                return NotFound();
//            }
//        }
//    }

//}

    [ApiController]
[Route("api/[controller]")]
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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersons()
    {
        try
        {
            _logger.LogInformation("Fetching all persons.");
            var persons = await _personService.GetAllPersonsAsync();
            return Ok(persons);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving persons.");
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while processing your request." });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetPerson(int id)
    {
        try
        {
            var person = await _personService.GetPersonByIdAsync(id);
            return Ok(person);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Person with ID {Id} not found.", id);
            return NotFound(new ErrorResponse { Message = $"Person with ID {id} not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching person with ID {Id}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while processing your request." });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonDto>> CreatePerson([FromBody] CreatePersonDto createPersonDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state on create person.");
            return BadRequest(new ErrorResponse { Message = "Invalid input." });
        }

        try
        {
            var person = await _personService.CreatePersonAsync(createPersonDto);
            return CreatedAtAction(nameof(GetPerson), new { id = person.PersonId }, person);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a person.");
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while processing your request." });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonDto>> UpdatePerson(int id, [FromBody] UpdatePersonDto updatePersonDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state on update person.");
            return BadRequest(new ErrorResponse { Message = "Invalid input." });
        }

        try
        {
            var updatedPerson = await _personService.UpdatePersonAsync(id, updatePersonDto);
            return Ok(updatedPerson);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Person with ID {Id} not found for update.", id);
            return NotFound(new ErrorResponse { Message = $"Person with ID {id} not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating person with ID {Id}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while processing your request." });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePerson(int id)
    {
        try
        {
            await _personService.DeletePersonAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Person with ID {Id} not found for deletion.", id);
            return NotFound(new ErrorResponse { Message = $"Person with ID {id} not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting person with ID {Id}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while processing your request." });
        }
    }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking existence of person with ID {Id}", id);
            return StatusCode(500, new ErrorResponse { Message = "An error occurred while processing your request." });
        }
    }
}
}

