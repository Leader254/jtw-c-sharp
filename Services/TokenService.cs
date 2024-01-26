using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using blog_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace blog_api.Services
{
    public class TokenService
    {
        private readonly int _tokenLifeTime = 30;
        private readonly ILogger<TokenService> _logger;

        public TokenService(ILogger<TokenService> logger)
        {
            _logger = logger;
        }

        public string CreateToken(ApplicationUser user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(_tokenLifeTime);

            var token = CreateJwtToken(CreateClaims(user), CreateSigningCredentials(), expiration);
            var tokenHandler = new JwtSecurityTokenHandler();
            _logger.LogInformation("Jwt Token Created");

            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials CreateSigningCredentials()
        {
            var symetricSecurityKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["SymmetricSecurityKey"];

            return new SigningCredentials
                (
                    new SymmetricSecurityKey
                    (
                        Encoding.UTF8.GetBytes(symetricSecurityKey)
                    ),
                    SecurityAlgorithms.HmacSha256
                );
        }

        private List<Claim> CreateClaims(ApplicationUser user)
{
    var jwtSub = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["JwtRegisteredClaimNamesSub"];

    try
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, jwtSub),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id?.ToString() ?? ""),
            new Claim(ClaimTypes.Name, user.UserName?.ToString() ?? ""),
            new Claim(ClaimTypes.Email, user.Email?.ToString() ?? ""),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        return claims;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
}


        private JwtSecurityToken CreateJwtToken(
            List<Claim> claims,
            SigningCredentials credentials,
            DateTime expiration
        ) =>
            new(
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                    .GetSection("JwtTokenSettings")["ValidIssuer"],
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                    .GetSection("JwtTokenSettings")["ValidAudience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );
    }
}
