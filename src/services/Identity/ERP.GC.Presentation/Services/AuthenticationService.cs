using ERP.GC.Presentation.Configuration;
using ERP.GC.Presentation.Models;
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
        SignInManager<Usuario> SignInManager { get; }
        UserManager<Usuario> UserManager { get; }

        Task<string> GenerateAccessTokenAsync(Usuario user);
        Task<string> GenerateRefreshTokenAsync(Usuario user);
        bool ValidateRefreshToken(string refreshToken, out JwtSecurityToken validatedToken);
    }


    public class AuthenticationService : IAuthenticationService
    {
        public SignInManager<Usuario> SignInManager { get; }
        public UserManager<Usuario> UserManager { get; }

        private readonly JwtConfigSettings _JwtConfigSettings;

        public AuthenticationService(SignInManager<Usuario> signInManager,
                                     UserManager<Usuario> userManager,
                                     IOptions<JwtConfigSettings> jwtConfigSettings)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            _JwtConfigSettings = jwtConfigSettings.Value;
        }

        public async Task<string> GenerateAccessTokenAsync(Usuario usuario)
        {
            var claims = await UserManager.GetClaimsAsync(usuario);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, usuario.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            claims.Add(new Claim("cargo", usuario.Cargo.ToString()));
            claims.Add(new Claim("empresaId", usuario.EmpresaId.ToString()));

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

        public async Task<string> GenerateRefreshTokenAsync(Usuario usuario)
        {
            var jti = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
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

            await UpdateLastGeneratedRtClaim(usuario, jti);
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

        private async Task UpdateLastGeneratedRtClaim(Usuario usuario, string jti)
        {
            var claims = await UserManager.GetClaimsAsync(usuario);
            var newLastRtClaim = new Claim("lastRefreshToken", jti);

            var claimLastRt = claims.FirstOrDefault(f => f.Type == "lastRefreshToken");

            if (claimLastRt != null)
                await UserManager.ReplaceClaimAsync(usuario, claimLastRt, newLastRtClaim);
            else
                await UserManager.AddClaimAsync(usuario, newLastRtClaim);
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
