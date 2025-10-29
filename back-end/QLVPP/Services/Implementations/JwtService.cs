using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using QLVPP.DTOs;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private const string _refreshToken = "REFRESH_TOKEN";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public JwtService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<string> GenerateAccessTokenAsync(Employee employee, HttpResponse response)
        {
            var jwtKey =
                _configuration["Jwt:Key"]
                ?? throw new ArgumentNullException("JWT_KEY is missing. Check .env file.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("id", employee.Id.ToString()),
                new Claim("name", employee.Name ?? ""),
                new Claim("account", employee.Account ?? ""),
                new Claim("warehouseId", employee.WarehouseId.ToString() ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var jwtIssuer =
                _configuration["Jwt:Issuer"]
                ?? throw new ArgumentNullException("JWT_ISSUER is missing. Check .env file.");
            var jwtAudience =
                _configuration["Jwt:Audience"]
                ?? throw new ArgumentNullException("JWT_AUDIENCE is missing. Check .env file.");
            var jwtExpires = int.TryParse(_configuration["Jwt:ExpireMinutes"], out var expires)
                ? expires
                : 30;

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtExpires),
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
                EmployeeId = employee.Id,
            };

            await _unitOfWork.RefreshToken.Add(refreshTokenEntity);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenEntity.ExpiredAt,
            };
            response.Cookies.Append(_refreshToken, refreshToken, cookieOptions);

            await _unitOfWork.SaveChanges();
            return accessToken;
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
            var secretKey =
                jwtSettings["Key"]
                ?? throw new ArgumentNullException("JwtSettings:Key is missing in configuration");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out _
                );
                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                try
                {
                    tokenHandler.ValidateToken(
                        token,
                        new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateIssuer = true,
                            ValidIssuer = issuer,
                            ValidateAudience = true,
                            ValidAudience = audience,
                            ValidateLifetime = false,
                            ClockSkew = TimeSpan.Zero,
                        },
                        out _
                    );

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

        public async Task<string> RenewAccessTokenAsync(HttpRequest request, HttpResponse response)
        {
            var refreshToken = request.Cookies[_refreshToken];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Invalid refresh token");
            var refreshTokenEntity = await _unitOfWork.RefreshToken.GetByTokenAsync(refreshToken);
            if (
                refreshTokenEntity == null
                || refreshTokenEntity.IsRevoked
                || refreshTokenEntity.IsUsed
                || refreshTokenEntity.ExpiredAt <= DateTime.Now
            )
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var accessToken = request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(accessToken) || !Validator(accessToken))
                throw new UnauthorizedAccessException("Invalid access token");

            var claims = GetClaims(accessToken);
            var userIdStr = claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Invalid access token");

            refreshTokenEntity.IsUsed = true;
            refreshTokenEntity.IsRevoked = true;

            await _unitOfWork.RefreshToken.Update(refreshTokenEntity);

            var employee = await _unitOfWork.Employee.GetById(userId);
            if (employee == null)
                throw new KeyNotFoundException("User does not exist");

            await _unitOfWork.SaveChanges();

            return await GenerateAccessTokenAsync(employee, response);
        }

        public async Task RevokeTokenAsync(
            string accessToken,
            HttpRequest request,
            HttpResponse response
        )
        {
            var refreshToken = request.Cookies[_refreshToken];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is missing or invalid.");

            var refreshTokenEntity = await _unitOfWork.RefreshToken.GetByTokenAsync(refreshToken);
            if (refreshTokenEntity == null)
                throw new Exception("Refresh token does not exist.");
            if (refreshTokenEntity.IsRevoked)
                throw new Exception("Refresh token has already been revoked.");

            refreshTokenEntity.IsUsed = true;
            refreshTokenEntity.IsRevoked = true;
            await _unitOfWork.RefreshToken.Update(refreshTokenEntity);

            response.Cookies.Delete(_refreshToken);

            var claims = GetClaims(accessToken);
            var account = claims.FirstOrDefault(c => c.Type == "account")?.Value ?? "unknown";
            var jti = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expUnix = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expUnix))
                throw new Exception("Access token missing required claims.");

            if (!long.TryParse(expUnix, out var expSeconds))
                throw new Exception("Invalid expiration claim in token.");

            var expiration = DateTimeOffset.FromUnixTimeSeconds(expSeconds).DateTime;

            var invalidToken = new InvalidToken
            {
                Jti = jti,
                Expiration = expiration,
                RevokedAt = DateTime.Now,
                RevokedBy = account,
            };

            await _unitOfWork.InvalidToken.Add(invalidToken);
            await _unitOfWork.SaveChanges();
        }
    }
}
