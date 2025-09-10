using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace PrototipoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<AppUser> userManager, TokenService tokenService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized("Credenciales inválidas");

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken(GetIpAddress());

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            SetRefreshTokenCookie(refreshToken.Token, refreshToken.ExpiresAt);

            return Ok(new { AccessToken = accessToken });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = new AppUser
            {
                UserName = request.UserName ?? request.Email,
                Email = request.Email,
                EmailConfirmed = true, // No hace falta confirmar el email
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Asegura que el rol 'Admin' existe
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            // Asignar rol de administrador por defecto
            await _userManager.AddToRoleAsync(user, "Admin");

            return Ok(new { Message = "Usuario registrado correctamente" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No refresh token provided");

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow));

            if (user == null)
                return Unauthorized("Refresh token inválido o expirado");

            var tokenEntity = user.RefreshTokens.First(rt => rt.Token == refreshToken);
            tokenEntity.IsRevoked = true;

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken(GetIpAddress());
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);
            SetRefreshTokenCookie(newRefreshToken.Token, newRefreshToken.ExpiresAt);

            return Ok(new { AccessToken = newAccessToken });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null)
                return Unauthorized();

            // Revoca todos los refresh tokens activos
            foreach (var rt in user.RefreshTokens.Where(rt => !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow))
                rt.IsRevoked = true;
            await _userManager.UpdateAsync(user);

            // Elimina la cookie
            Response.Cookies.Delete("refreshToken");
            return Ok(new { Message = "Sesión cerrada correctamente" });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users
                .Select(u => new { u.Id, u.UserName, u.Email })
                .ToListAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpDelete("user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string? id, [FromQuery] string? email, [FromQuery] string? userName)
        {
            AppUser? user = null;
            if (!string.IsNullOrEmpty(id))
                user = await _userManager.FindByIdAsync(id);
            else if (!string.IsNullOrEmpty(email))
                user = await _userManager.FindByEmailAsync(email);
            else if (!string.IsNullOrEmpty(userName))
                user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound("Usuario no encontrado");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Usuario eliminado correctamente" });
        }

        private void SetRefreshTokenCookie(string token, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,           // Previene acceso desde JavaScript
                Secure = true,             // Solo HTTPS
                SameSite = SameSiteMode.None, // Usa None si frontend y backend están en dominios distintos
                Path = "/api/auth",      // Limita scope a endpoints auth
                Expires = expires,         // Expiración igual al refresh token
                IsEssential = true
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"]!;
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        }
    }
}
