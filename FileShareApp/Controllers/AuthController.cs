using FileShareApp.Backend.Models;
using FileShareApp.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileShareApp.Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto dto)
    {
        var result = await _authService.RegisterAsync(dto.Email, dto.Password);
        return result
            ? Ok()
            : BadRequest();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto dto)
    {
        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        return token != null
            ? Ok(new { token})
            : Unauthorized();
    }
}