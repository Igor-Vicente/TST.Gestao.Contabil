using ERP.Registro.Presentation.Services;
using ERP.Registro.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace ERP.Registro.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/registro")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(LoginViewModel model)
        {
            var result = await _authenticationService.SignInManager.PasswordSignInAsync(model.Email, model.Senha, false, true);

            if (result.Succeeded)
            {
                var usuario = await _authenticationService.UserManager.FindByEmailAsync(model.Email);
                var accessToken = await _authenticationService.GenerateAccessTokenAsync(usuario);
                var refreshToken = await _authenticationService.GenerateRefreshTokenAsync(usuario);

                return Ok(new { ac = accessToken, rf = refreshToken, email = usuario.Email });
            }

            if (result.IsLockedOut)
                return BadRequest("Usuário bloqueado temporariamente devido a tentativas inválidas.");

            return BadRequest("Nome de usuário ou senha inválidos");
        }


        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenViewModel model)
        {
            if (!_authenticationService.ValidateRefreshToken(model.RefreshToken, out var validatedToken))
                return BadRequest("Token Inválido");

            var jti = validatedToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var email = validatedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

            var usuario = await _authenticationService.UserManager.FindByEmailAsync(email);
            var userClaims = await _authenticationService.UserManager.GetClaimsAsync(usuario);

            if (!userClaims.Any(c => c.Type == "lastRefreshToken" && c.Value == jti))
                return BadRequest("Token Expirado");

            if (usuario.LockoutEnabled)
                if (usuario.LockoutEnd < DateTime.Now)
                    return BadRequest("Usuário temporariamente bloqueado");

            var accessToken = await _authenticationService.GenerateAccessTokenAsync(usuario);
            var refreshToken = await _authenticationService.GenerateRefreshTokenAsync(usuario);

            return Ok(new { ac = accessToken, rf = refreshToken, email = usuario.Email });
        }
    }
}
