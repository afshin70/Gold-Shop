using Gold.SharedKernel.DTO.JWT;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gold.Infrastracture.Configurations.JWTSystem
{
    public class JwtTokenService
    {
        //private const double EXPIRY_DURATION_MINUTES = 30;

        public string BuildToken(string key, string issuer, DateTime expireDate, List<Claim> userClaims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, userClaims, expires: expireDate, signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public string BuildToken( DateTime expireDate, List<Claim> userClaims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtAuthOption.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(JwtAuthOption.Issuer, JwtAuthOption.Audience, userClaims, expires: expireDate, signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public bool IsValid(string token)
        {
            var secretKey = Encoding.UTF8.GetBytes(JwtAuthOption.Key);
            var securityKey = new SymmetricSecurityKey(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
              var result=  tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = JwtAuthOption.ValidateIssuerSigningKey,
                    ValidateIssuer = JwtAuthOption.ValidateIssuer,
                    ValidateAudience = JwtAuthOption.ValidateAudience,
                    ValidateLifetime= JwtAuthOption.ValidateLifetime,
                    ValidIssuer = JwtAuthOption.Issuer,
                    ValidAudience = JwtAuthOption.Audience,
                    IssuerSigningKey = securityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool IsValid(string key, string issuer, string token)
        {
            var secretKey = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = securityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}