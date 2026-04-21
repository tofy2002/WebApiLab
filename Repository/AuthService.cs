using Lab2.DTOs.StudentDTOS;
using Lab3.DTOs;
using Lab3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Lab2.Repository
{
    public interface IAuthService
    {
        Task<(bool success, string message)> RegisterAsync(RegisterDTO dto);
        Task<(bool success, string message, AuthResponseDTO? response)> LoginAsync(LoginDTO dto);
        Task<(bool success, string message, AuthResponseDTO? response)> RefreshTokenAsync(string refreshToken);
        Task<(bool success, string message)> RevokeTokenAsync(string refreshToken);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ITIDbContext _context;

        public AuthService(
            IConfiguration config,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITIDbContext context)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<(bool success, string message)> RegisterAsync(RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.username,
                Email = dto.email,
                FullName = dto.fullname,
                age = dto.age
            };

            var result = await _userManager.CreateAsync(user, dto.password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            if (!await _roleManager.RoleExistsAsync("Student"))
                await _roleManager.CreateAsync(new IdentityRole("Student"));

            await _userManager.AddToRoleAsync(user, "Student");

            return (true, "User created successfully");
        }
        public async Task<(bool success, string message, AuthResponseDTO? response)> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.username);
            if (user == null)
                return (false, "Invalid username or password", null);

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.password, lockoutOnFailure: true);
            if (!result.Succeeded)
                return (false, "Invalid username or password", null);

            var roles = await _userManager.GetRolesAsync(user);

            var (jwtToken, jwtExpiry) = GenerateJwtToken(user, roles);
            var refreshToken = await CreateAndSaveRefreshTokenAsync(user);

            return (true, "Login successful", new AuthResponseDTO
            {
                Token = jwtToken,
                TokenExpiresAt = jwtExpiry,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresOn
            });
        }

        public async Task<(bool success, string message, AuthResponseDTO? response)> RefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(t => t.User)
                .SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null)
                return (false, "Invalid refresh token", null);

            if (!token.IsActive)
            {
                var reason = token.IsExpired ? "Refresh token has expired" : "Refresh token has been revoked";
                return (false, reason, null);
            }

            // Revoke old token (rotation — each refresh token is single-use)
            token.RevokedOn = DateTime.UtcNow;

            var user = token.User;
            var roles = await _userManager.GetRolesAsync(user);

            var (jwtToken, jwtExpiry) = GenerateJwtToken(user, roles);
            var newRefreshToken = await CreateAndSaveRefreshTokenAsync(user);

            return (true, "Token refreshed", new AuthResponseDTO
            {
                Token = jwtToken,
                TokenExpiresAt = jwtExpiry,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresOn
            });
        }
        public async Task<(bool success, string message)> RevokeTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null)
                return (false, "Invalid refresh token");

            if (!token.IsActive)
                return (false, "Token is already inactive");

            token.RevokedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return (true, "Token revoked successfully");
        }
        private (string token, DateTime expiry) GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name,             user.UserName!),
                new Claim(ClaimTypes.Email,            user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
        }
        private async Task<RefreshToken> CreateAndSaveRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateSecureToken(),
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
        private static string GenerateSecureToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}