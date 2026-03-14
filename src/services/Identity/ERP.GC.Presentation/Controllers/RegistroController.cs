using ERP.GC.Presentation.Abstractions;
using ERP.GC.Presentation.Commands;
using ERP.GC.Presentation.Services;
using ERP.GC.Presentation.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using static ERP.GC.Presentation.Configuration.CustomAuthorize;

namespace ERP.GC.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/registro")]
    public class RegistroController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMediator _mediator;
        private readonly INotificador _notificador;

        public RegistroController(IAuthenticationService authenticationService,
                                  IMediator mediator,
                                  INotificador notificador)
        {
            _authenticationService = authenticationService;
            _mediator = mediator;
            _notificador = notificador;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(LoginViewModel model)
        {
            var result = await _authenticationService.SignInManager.PasswordSignInAsync(model.Email, model.Senha, false, true);

            if (result.Succeeded)
            {
                var identityUser = await _authenticationService.UserManager.FindByEmailAsync(model.Email);
                var accessToken = await _authenticationService.GenerateAccessTokenAsync(identityUser);
                var refreshToken = await _authenticationService.GenerateRefreshTokenAsync(identityUser);

                return Ok(new { ac = accessToken, rf = refreshToken, email = identityUser.Email });
            }

            if (result.IsLockedOut)
                return BadRequest("Usuário bloqueado temporariamente devido a tentativas inválidas.");

            return BadRequest("Nome de usuário ou senha inválidos");
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenViewModel model)
        {

            if (!_authenticationService.ValidateRefreshToken(model.RefreshToken, out var validatedToken))
                return BadRequest("Token Inválido");

            var jti = validatedToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var email = validatedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

            var identityUser = await _authenticationService.UserManager.FindByEmailAsync(email);
            var userClaims = await _authenticationService.UserManager.GetClaimsAsync(identityUser);

            if (!userClaims.Any(c => c.Type == "lastRefreshToken" && c.Value == jti))
                return BadRequest("Token Expirado");

            if (identityUser.LockoutEnabled)
                if (identityUser.LockoutEnd < DateTime.Now)
                    return BadRequest("Usuário temporariamente bloqueado");

            var accessToken = await _authenticationService.GenerateAccessTokenAsync(identityUser);
            var refreshToken = await _authenticationService.GenerateRefreshTokenAsync(identityUser);

            return Ok(new { ac = accessToken, rf = refreshToken, email = identityUser.Email });
        }


        [HttpPost("usuario")]
        [ClaimsAuthorize("isAdmin", "true")]
        public async Task<IActionResult> RegistroUsuario(RegistroUsuarioViewModel model)
        {
            var identityUser = new IdentityUser<int> { Email = model.Email, UserName = model.Email };

            var result = await _authenticationService.UserManager.CreateAsync(identityUser, model.Senha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var usuario = await _mediator.Send(
                new AdicionarUsuarioCommand(identityUser.Id, model.Nome, identityUser.Email, model.EmpresaId));

            if (_notificador.TemNotificacoes())
            {
                await _authenticationService.UserManager.DeleteAsync(identityUser);
                return BadRequest(_notificador.ObterNotificacoes());
            }

            var accessToken = await _authenticationService.GenerateAccessTokenAsync(identityUser);
            var refreshToken = await _authenticationService.GenerateRefreshTokenAsync(identityUser);

            return Ok(new { ac = accessToken, rf = refreshToken, email = usuario.Email });
        }


        [HttpPost("empresa")]
        [ClaimsAuthorize("isAdmin", "true")]
        public async Task<IActionResult> RegistroEmpresa(RegistroEmpresaViewModel model)
        {
            var empresa = await _mediator.Send(
                new AdicionarEmpresaCommand(model.Cnpj, model.RazaoSocial, model.NomeFantasia, model.RegimeTributario, model.Email));

            if (_notificador.TemNotificacoes())
                return BadRequest(_notificador.ObterNotificacoes());


            return Ok(empresa);
        }
    }
}
