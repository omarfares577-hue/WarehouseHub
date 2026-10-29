using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WarehouseHub.Core.Contracts;
using WarehouseHub.Core.Dtos.Identity;
using WarehouseHub.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WarehouseHub.Core.Errors;
namespace WarehouseHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        public  AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenServices tokenServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto Dto)
        {
            var existingemail = await _userManager.FindByEmailAsync(Dto.Email);
            if (existingemail is not null)
            { return BadRequest(); }
            var user = new AppUser
            {
                UserName = Dto.Email,
                Email = Dto.Email,
                DisplayName = Dto.Name,
                PhoneNumber = Dto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, Dto.Password);
            if (!result.Succeeded)
                return BadRequest();
            return Ok(new UserDto
            {
                Email = user.Email,
                Name = user.DisplayName,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager)
            });
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto Dto)
        {
            var user = await _userManager.FindByEmailAsync(Dto.Email);
            if (user is null)
                return Unauthorized();
            var result = await _signInManager.CheckPasswordSignInAsync(user, Dto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }
            return Ok(new UserDto
            {
                Email = user.Email!,
                Name = user.DisplayName,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager)
            });
        }
        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email!);
            return Ok(new UserDto
            {
                Email = user!.Email!,
                Name = user.DisplayName,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager)
            });
        }
    }
}
