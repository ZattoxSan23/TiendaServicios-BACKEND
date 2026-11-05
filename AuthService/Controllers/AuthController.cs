using AuthService.DTOs;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthServiceClass _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthServiceClass authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto dto)
        {
            try
            {
                var result = _authService.Register(dto);
                return Ok(new { message = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { error = "Usuario y contraseña son requeridos" });
                }

                var token = _authService.Login(request);

                if (token == null)
                {
                    return Unauthorized(new { error = "Credenciales inválidas" });
                }

                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}