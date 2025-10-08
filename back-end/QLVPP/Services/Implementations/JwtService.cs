using Microsoft.IdentityModel.Tokens;
using QLVPP.DTOs;
using QLVPP.Models;
using QLVPP.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QLVPP.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public JwtService (IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<TokenDto> GenerateAccessTokenAsync(Employee employee)
        {
            var jwtSettings = _configuration.GetSection("JwtSec");
            var secretKey = jwtSettings["Key"] ?? throw new ArgumentNullException("JwtSettings:Key is missing in configuration");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("id", employee.Id.ToString()),
                new Claim("name", employee.Name ?? ""),
                new Claim("account", employee.Account ?? ""),
                new Claim("warehouseId", employee.WarehouseId.ToString() ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var expiresInMinutes = jwtSettings.GetValue<int>("ExpiresInMinutes");
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiresInMinutes),
                signingCredentials: credentials
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                JwtId = token.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddDays(7),
                EmployeeId = employee.Id
            };

            await _unitOfWork.RefreshToken.Add(refreshTokenEntity);
            await _unitOfWork.SaveChanges();
            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public bool Validator(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSec");
            var secretKey = jwtSettings["Key"] ?? throw new ArgumentNullException("JwtSettings:Key is missing in configuration");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);
                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero
                    }, out _);

                    return true;
                }
                catch
                {
                    return false; 
                }
            }
            catch
            {
                return false;
            }
        }
        public IEnumerable<Claim> GetClaims(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);

            return jwtToken.Claims;
        }

        public async Task<TokenDto> RenewAccessTokenAsync(TokenDto request)
        {
            var refreshTokenEntity = await _unitOfWork.RefreshToken.GetByTokenAsync(request.RefreshToken);
            if (refreshTokenEntity == null ||
                refreshTokenEntity.IsRevoked ||
                refreshTokenEntity.IsUsed ||
                refreshTokenEntity.ExpiredAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (!Validator(request.AccessToken))
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var claims = GetClaims(request.AccessToken);
            var userIdStr = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            refreshTokenEntity.IsUsed = true;
            refreshTokenEntity.IsRevoked = true;
            await _unitOfWork.RefreshToken.Update(refreshTokenEntity);

            var employee = await _unitOfWork.Employee.GetById(userId);
            if (employee == null)
                throw new KeyNotFoundException("User does not exist");

            await _unitOfWork.SaveChanges();

            return await GenerateAccessTokenAsync(employee);
        }

        public async Task RevokeTokenAsync(TokenDto request)
        {
            var refreshTokenEntity = await _unitOfWork.RefreshToken.GetByTokenAsync(request.RefreshToken);
            if (refreshTokenEntity == null)
                throw new Exception("Refresh token does not exist.");
            if (refreshTokenEntity.IsRevoked)
                throw new Exception("Refresh token has already been revoked.");

            refreshTokenEntity.IsUsed = true;
            refreshTokenEntity.IsRevoked = true;
            await _unitOfWork.RefreshToken.Update(refreshTokenEntity);

            var claims = GetClaims(request.AccessToken);
            var account = claims.FirstOrDefault(c => c.Type == "account")?.Value ?? "unknown";
            var jti = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expUnix = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expUnix))
                throw new Exception("Access token missing required claims.");

            if (!long.TryParse(expUnix, out var expSeconds))
                throw new Exception("Invalid expiration claim in token.");

            var expiration = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

            var invalidToken = new InvalidToken
            {
                Jti = jti,
                Expiration = expiration,
                RevokedAt = DateTime.UtcNow,
                RevokedBy = account
            };

            await _unitOfWork.InvalidToken.Add(invalidToken);
            await _unitOfWork.SaveChanges();
        }
    }
}
