using Microsoft.AspNetCore.Mvc;
using WeatherAPI.WeatherDomain.Core.Services;
using WeatherAPI.WeatherDomain.Dtos;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            return Ok(await _authService.Register(request));    
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            return Ok(await _authService.Login(request));
        }

    }
}
