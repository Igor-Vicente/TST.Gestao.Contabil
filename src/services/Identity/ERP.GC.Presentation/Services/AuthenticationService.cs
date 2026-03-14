using ERP.GC.Presentation.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERP.GC.Presentation.Services
{
    public interface IAuthenticationService
    {
        SignInManager<IdentityUser<int>> SignInManager { get; }
        UserManager<IdentityUser<int>> UserManager { get; }

        Task<string> GenerateAccessTokenAsync(IdentityUser<int> user);
        Task<string> GenerateRefreshTokenAsync(IdentityUser<int> user);
        bool ValidateRefreshToken(string refreshToken, out JwtSecurityToken validatedToken);
    }


    public class AuthenticationService : IAuthenticationService
    {
        public SignInManager<IdentityUser<int>> SignInManager { get; }
        public UserManager<IdentityUser<int>> UserManager { get; }

        private readonly JwtConfigSettings _JwtConfigSettings;

        public AuthenticationService(SignInManager<IdentityUser<int>> signInManager,
                                     UserManager<IdentityUser<int>> userManager,
                                     IOptions<JwtConfigSettings> jwtConfigSettings)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            _JwtConfigSettings = jwtConfigSettings.Value;
        }

        public async Task<string> GenerateAccessTokenAsync(IdentityUser<int> user)
        {
            var claims = await UserManager.GetClaimsAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _JwtConfigSettings.Issuer,
                Audience = _JwtConfigSettings.Audience,
                SigningCredentials = GetCurrentSigningCredentials(),
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(60),
                IssuedAt = DateTime.UtcNow,
                TokenType = "at+jwt"
            });

            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(IdentityUser<int> user)
        {
            var jti = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _JwtConfigSettings.Issuer,
                Audience = _JwtConfigSettings.Audience,
                SigningCredentials = GetCurrentSigningCredentials(),
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddDays(7),
                TokenType = "rt+jwt"
            });

            await UpdateLastGeneratedRtClaim(user, jti);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateRefreshToken(string refreshToken, out JwtSecurityToken validatedToken)
        {
            try
            {
                var validationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = _JwtConfigSettings.Issuer,
                    ValidAudience = _JwtConfigSettings.Audience,
                    RequireSignedTokens = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_JwtConfigSettings.SecretKey)),
                };

                new JwtSecurityTokenHandler().ValidateToken(refreshToken, validationParameters, out var securityToken);
                validatedToken = (JwtSecurityToken)securityToken;
                return true;
            }
            catch
            {
                validatedToken = null;
                return false;
            }
        }

        private async Task UpdateLastGeneratedRtClaim(IdentityUser<int> user, string jti)
        {
            var claims = await UserManager.GetClaimsAsync(user);
            var newLastRtClaim = new Claim("lastRefreshToken", jti);

            var claimLastRt = claims.FirstOrDefault(f => f.Type == "lastRefreshToken");

            if (claimLastRt != null)
                await UserManager.ReplaceClaimAsync(user, claimLastRt, newLastRtClaim);
            else
                await UserManager.AddClaimAsync(user, newLastRtClaim);
        }

        private SigningCredentials GetCurrentSigningCredentials()
        {
            var secretKey = Encoding.ASCII.GetBytes(_JwtConfigSettings.SecretKey);
            return new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
