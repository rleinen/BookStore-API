using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILoggerService _logger;

        public UsersController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, 
            IConfiguration config,
            ILoggerService logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Register Endpoint
        /// </summary>
        /// <param name="userDtO"></param>
        /// <returns></returns>
        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            try
            {
                var username = userDTO.Email;
                var password = userDTO.Password;
                var user = new IdentityUser { Email = username, UserName = username };
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    _logger.LogError("+++ User Register Failed +++");
                    foreach(var error in result.Errors)
                    {
                        _logger.LogError($"{error.Code} >>> {error.Description} ");
                    }
                    return StatusCode(500, "Oops! Something went wrong!");
                }

                return Ok(new { result.Succeeded });
            }
            catch (Exception e)
            {
                return LogError(e, "Register Failed!");
            }

        }


        /// <summary>
        /// User Login Endpoint
        /// </summary>
        /// <param name="userDtO"></param>
        /// <returns></returns>
        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(userDTO.Email);
                var tokenString = GenerateJSONWebToken(user);
                return Ok( new { token = tokenString });
            }

            return Unauthorized(userDTO);
        }

        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                notBefore: null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
