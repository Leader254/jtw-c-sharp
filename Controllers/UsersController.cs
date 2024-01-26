using blog_api.Context;
using blog_api.Models;
using blog_api.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace blog_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, TokenService tokenService, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegistrationRequest payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userManager.CreateAsync(
                new ApplicationUser { UserName = payload.Username, Email = payload.Email, Role = payload.Role },
                payload.Password!
                );
            if (result.Succeeded)
            {
                payload.Password = "";
                return CreatedAtAction(nameof(Register), new { email = payload.Email, role = payload.Role }, payload);
            }
            return BadRequest(ModelState);
        }
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest loginPayload)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var managedUser = await _userManager.FindByEmailAsync(loginPayload.Email!);
            if(managedUser == null)
            {
                return BadRequest("Invalid Credentials");
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(managedUser, loginPayload.Password)!;
            if (!isValidPassword)
            {
                return BadRequest("Invalid Credentials");
            }

            var userInDb = _context.Users.FirstOrDefault(u => u.Email == loginPayload.Email);
            if (userInDb == null)
            {
                return Unauthorized();
            }

            var accessToken = _tokenService.CreateToken(userInDb);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                Username = userInDb.UserName,
                Email = userInDb.Email,
                Token = accessToken
            });
        }
    }
}
