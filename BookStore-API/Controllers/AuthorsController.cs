using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{        
    /// <summary>
    /// Interacts with the Authors of Books
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all Authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempted GetAUthors...");
                var authors = await _authorRepository.FindAll();
                var res = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Success!");
                return Ok(res);
            }
            catch (Exception e)
            {
                return LogError(e, "GetAuthors Failed!");
            }

        }

        /// <summary>
        /// Get an Author by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An Author</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted GetAuthor... for id: {id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"No author with Id: {id} not found.");
                    return NotFound();
                }
                var res = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo("Success!");
                return Ok(res);
            }
            catch (Exception e)
            {
                return LogError(e, "GetAuthor Failed!");
            }

        }

        /// <summary>
        /// Create Author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns>Created Author object</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Attempted Create Author...");
                if (authorDTO == null || !ModelState.IsValid)
                {
                    _logger.LogError("Bad Author submission!");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var success = await _authorRepository.Create(author);
                if (!success)
                {
                    _logger.LogError("Create Failed!");
                    return BadRequest(ModelState);
                }

                _logger.LogInfo("Success!");
                return Created("Author Created!", new { author });
            }
            catch (Exception e)
            {
                return LogError(e, "CreateAuthor Failed!");
            }

        }

        /// <summary>
        /// Update Author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Attempted Update Author...");

                var exists = await _authorRepository.Exists(id);
                if (exists)
                {
                    _logger.LogError("Author not found to update!");
                    return BadRequest(ModelState);
                }

                if (id < 1 || authorDTO == null || ModelState.IsValid == false)
                {
                    _logger.LogError("Bad Author submission!");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var success = await _authorRepository.Update(author);
                if (!success)
                {
                    _logger.LogError("Update Failed!");
                    return BadRequest(ModelState);
                }

                _logger.LogInfo("Success!");
                return NoContent();
            }
            catch (Exception e)
            {
                return LogError(e, "UpdateAuthor Failed!");
            }

        }

        /// <summary>
        /// Delete an Author by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted Delete Author...");
                if (id < 1)
                {
                    _logger.LogError("Bad Author Id!");
                    return BadRequest();
                }

                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"No author with Id: {id} not found.");
                    return NotFound();
                }

                var success = await _authorRepository.Delete(author);
                if (!success)
                {
                    _logger.LogError("Delete Failed!");
                    return StatusCode(500,"Something failed with Delete Author");
                }

                _logger.LogInfo("Success!");
                return Ok("Success");
            }
            catch (Exception e)
            {
                return LogError(e, "GetAuthor Failed!");
            }

        }


        private ObjectResult LogError(Exception e, string msg)
        {
            _logger.LogError($"{e.Message} - {e.InnerException}");
            return StatusCode(500, "Oops! Something went wrong!");
        }


    }
}
