using BilliardServer.Core.Models;
using BilliardServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BilliardServer.Infrastructure
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<UserEntity> _userManager;

        public TokenService(IConfiguration config, UserManager<UserEntity> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<TokenData> GenerateAccessToken(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenExpires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:AccessTokenLifetimeMinutes"]!));
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: tokenExpires,
                signingCredentials: creds);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpires = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenLifetimeDays"]!));

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new() {  
                Token = tokenString, 
                TokenExpiredTimestamp = ((DateTimeOffset)tokenExpires.ToUniversalTime()).ToUnixTimeSeconds(), 
                RefreshToken = refreshToken, 
                RefreshTokenExpiredTimestamp = ((DateTimeOffset)refreshTokenExpires.ToUniversalTime()).ToUnixTimeSeconds()
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
