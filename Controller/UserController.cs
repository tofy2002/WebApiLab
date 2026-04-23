using Lab2.DTOs.StudentDTOS;
using Lab3.DTOs;
using Lab3.Models;
using Lab3.Services;
using Lab3.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

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
             await _service.RegisterAsync(dto);
            

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User registered successfully",
                Data= null
            });
        }
        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var result = await _service.LoginAsync(login);
            

            return Ok(new ApiResponse<AuthResponseDTO>
            {
                Success = true,
                Message = "Login successful",
                Data = result
            });
        }

    
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _service.RefreshTokenAsync(refreshToken);
           

            return Ok(new ApiResponse<AuthResponseDTO>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = result
            });
        }

        // POST api/User/RevokeToken
        [HttpPost("RevokeToken")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _service.RevokeTokenAsync(userId!, refreshToken);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Token revoked",
                Data = null
            });
        }
    }
}