using Lab2.DTOs.StudentDTOS;
using Lab2.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _service;

        public UserController(IAuthService authService)
        {
            _service = authService;
        }

        // POST api/User/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var result = await _service.RegisterAsync(dto);
            if (!result.success)
                return BadRequest(result.message);

            return Ok(result.message);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var result = await _service.LoginAsync(login);
            if (!result.success)
                return Unauthorized(result.message);

            return Ok(result.response);
        }

    
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _service.RefreshTokenAsync(refreshToken);
            if (!result.success)
                return Unauthorized(result.message);

            return Ok(result.response);
        }

        // POST api/User/RevokeToken
        [HttpPost("RevokeToken")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
        {
            var result = await _service.RevokeTokenAsync(refreshToken);
            if (!result.success)
                return BadRequest(result.message);

            return Ok(result.message);
        }
    }
}