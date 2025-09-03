using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace whris.UI.Services
{
    public static class TokenService
    {
        private static string secretKey = "supersecretkey12345"; // move to appsettings.json later

        public static string GenerateToken(int employeeId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employeeId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static int? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var employeeId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.Parse(employeeId);
            }
            catch
            {
                return null; // invalid or expired
            }
        }

    }
}
