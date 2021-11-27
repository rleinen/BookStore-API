using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Book Endpoints
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public BooksController(IBookRepository bookRepository, ILoggerService logger, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Books
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                _logger.LogInfo("Attempted GetBooks...");
                var books = await _bookRepository.FindAll();
                var res = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo("Success!");
                return Ok(res);
            }
            catch (Exception e)
            {
                return LogError(e, "GetBooks Failed!");
            }

        }


        /// <summary>
        /// Get an Book by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A Book</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted GetBook... for id: {id}");
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"No book with Id: {id} not found.");
                    return NotFound();
                }
                var res = _mapper.Map<BookDTO>(book);
                _logger.LogInfo("Success!");
                return Ok(res);
            }
            catch (Exception e)
            {
                return LogError(e, "GetBook Failed!");
            }

        }

        /// <summary>
        /// Create Book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns>Created Book object</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDTO bookDTO)
        {
            try
            {
                _logger.LogInfo($"Attempted Create Book...");
                if (bookDTO == null || !ModelState.IsValid)
                {
                    _logger.LogError("Bad Book submission!");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookDTO);
                var success = await _bookRepository.Create(book);
                if (!success)
                {
                    _logger.LogError("Create Failed!");
                    return BadRequest(ModelState);
                }

                _logger.LogInfo("Success!");
                return Created("Book Created!", new { book });
            }
            catch (Exception e)
            {
                return LogError(e, "CreateBook Failed!");
            }

        }

        /// <summary>
        /// Update Book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            try
            {
                _logger.LogInfo($"Attempted Update Book...");

                var exists = await _bookRepository.Exists(id);
                if (exists == false)
                {
                    _logger.LogError("Book not found to update!");
                    return BadRequest(ModelState);
                }

                if (id < 1 || bookDTO == null || ModelState.IsValid == false)
                {
                    _logger.LogError("Bad Book submission!");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookDTO);
                var success = await _bookRepository.Update(book);
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
                return LogError(e, "UpdateBook Failed!");
            }

        }

        /// <summary>
        /// Delete an Book by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted Delete Book...");
                if (id < 1)
                {
                    _logger.LogError("Bad Book Id!");
                    return BadRequest();
                }

                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"No book with Id: {id} not found.");
                    return NotFound();
                }

                var success = await _bookRepository.Delete(book);
                if (!success)
                {
                    _logger.LogError("Delete Failed!");
                    return StatusCode(500, "Something failed with Delete Book");
                }

                _logger.LogInfo("Success!");
                return Ok("Success");
            }
            catch (Exception e)
            {
                return LogError(e, "GetBook Failed!");
            }

        }

        private ObjectResult LogError(Exception e, string msg)
        {
            var ctl = ControllerContext.ActionDescriptor.ControllerName;
            var act = ControllerContext.ActionDescriptor.ActionName;

            _logger.LogError($"{ctl}_{act}: {e.Message} - {e.InnerException}");
            return StatusCode(500, "Oops! Something went wrong!");
        }
    }
}
