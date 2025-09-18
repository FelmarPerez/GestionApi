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
                // Delegados / funciones anónimas
                Func<Task<bool>> userExists = async () =>
                    await _context.Usuario.AnyAsync(u => u.NombreUsuario == request.NombreUsuario || u.Email == request.Email);

                Func<string, string> hashPassword = pwd => _passwordService.HashPassword(pwd);
                Func<Usuario> buildUser = () => new Usuario
                {
                    NombreUsuario = request.NombreUsuario,
                    Email = request.Email,
                    Password = hashPassword(request.Password),
                    FechaCreacion = DateTime.Now,
                    Activo = true
                };

                Action<Usuario> addUser = u => _context.Usuario.Add(u);
                Func<Task> saveChanges = async () => await _context.SaveChangesAsync();
                Func<Usuario, string> genToken = u => _jwtService.GenerateToken(u);
                Func<string> genRefresh = () => _jwtService.GenerateRefreshToken();

                // Verificar si el usuario ya existe
                if (await userExists())
                {
                    return BadRequest(new { message = "El usuario o email ya existe" });
                }

                // Crear nuevo usuario
                var usuario = buildUser();

                addUser(usuario);
                await saveChanges();

                // Generar token
                var token = genToken(usuario);
                var refreshToken = genRefresh();

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
                // Delegados / funciones anónimas
                Func<Task<Usuario?>> findActiveByUsername = async () =>
                    await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario && u.Activo);
                Func<string, string, bool> verifyPassword = (plain, hash) => _passwordService.VerifyPassword(plain, hash);
                Func<Usuario, string> genToken = u => _jwtService.GenerateToken(u);
                Func<string> genRefresh = () => _jwtService.GenerateRefreshToken();

                // Buscar usuario
                var usuario = await findActiveByUsername();

                if (usuario == null || !verifyPassword(request.Password, usuario.Password))
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                // Generar token
                var token = genToken(usuario);
                var refreshToken = genRefresh();

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
