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

        public AuthController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
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

            SetRefreshTokenCookie(refreshToken.Token);

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
            SetRefreshTokenCookie(newRefreshToken.Token);

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

        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(int.Parse(HttpContext.RequestServices.GetService<IConfiguration>()!["Jwt:RefreshTokenDays"]!))
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
