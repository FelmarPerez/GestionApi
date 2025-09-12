using GestionApi.Data;
using GestionApi.Models;
using GestionApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GestionDBContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public AuthController(GestionDBContext context, IJwtService jwtService, IPasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Verificar si el usuario ya existe
                if (await _context.Usuario.AnyAsync(u => u.NombreUsuario == request.NombreUsuario || u.Email == request.Email))
                {
                    return BadRequest(new { message = "El usuario o email ya existe" });
                }

                // Crear nuevo usuario
                var usuario = new Usuario
                {
                    NombreUsuario = request.NombreUsuario,
                    Email = request.Email,
                    Password = _passwordService.HashPassword(request.Password),
                    FechaCreacion = DateTime.Now,
                    Activo = true
                };

                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();

                // Generar token
                var token = _jwtService.GenerateToken(usuario);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var response = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(60), // 1 hora
                    Usuario = new Usuario
                    {
                        Id = usuario.Id,
                        NombreUsuario = usuario.NombreUsuario,
                        Email = usuario.Email,
                        FechaCreacion = usuario.FechaCreacion,
                        Activo = usuario.Activo
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Buscar usuario
                var usuario = await _context.Usuario
                    .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario && u.Activo);

                if (usuario == null || !_passwordService.VerifyPassword(request.Password, usuario.Password))
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                // Generar token
                var token = _jwtService.GenerateToken(usuario);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var response = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(60), // 1 hora
                    Usuario = new Usuario
                    {
                        Id = usuario.Id,
                        NombreUsuario = usuario.NombreUsuario,
                        Email = usuario.Email,
                        FechaCreacion = usuario.FechaCreacion,
                        Activo = usuario.Activo
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                // En una implementación real, deberías validar el refresh token
                // Por simplicidad, aquí solo generamos un nuevo token
                // Deberías almacenar y validar refresh tokens en la base de datos

                return BadRequest(new { message = "Funcionalidad de refresh token no implementada completamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("validate")]
        public IActionResult ValidateToken()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Token no proporcionado" });
                }

                if (!_jwtService.ValidateToken(token))
                {
                    return Unauthorized(new { message = "Token inválido" });
                }

                var userId = _jwtService.GetUserIdFromToken(token);
                return Ok(new { message = "Token válido", userId = userId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}
