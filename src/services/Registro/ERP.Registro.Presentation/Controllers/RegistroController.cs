using ERP.Registro.Presentation.Abstractions;
using ERP.Registro.Presentation.Commands;
using ERP.Registro.Presentation.Configuration;
using ERP.Registro.Presentation.Data.Repositories;
using ERP.Registro.Presentation.Models;
using ERP.Registro.Presentation.Services;
using ERP.Registro.Presentation.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Identidade;

namespace ERP.Registro.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/registro")]
    public class RegistroController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMediator _mediator;
        private readonly INotificador _notificador;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthUser _authUser;

        public RegistroController(IAuthenticationService authenticationService,
                                  IMediator mediator,
                                  INotificador notificador,
                                  IUsuarioRepository usuarioRepository,
                                  IAuthUser authUser)
        {
            _authenticationService = authenticationService;
            _mediator = mediator;
            _notificador = notificador;
            _usuarioRepository = usuarioRepository;
            _authUser = authUser;
        }


        [HttpPost("usuario")]
        [Authorize(Policy = "PodeCriarUsuario")]
        public async Task<IActionResult> RegistroUsuario(RegistroUsuarioViewModel model)
        {
            if (model.Cargo == Cargo.AdministradorGeral) return Forbid();

            var cargo = _authUser.ObterCargoUsuario();
            var empresaId = _authUser.ObterEmpresaIdUsuario();

            // Gestor só pode criar usuários para a própria empresa e não pode criar AdministradorGeral.
            if (string.Equals(cargo, Cargo.Gestor.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (model.Cargo == Cargo.AdministradorGeral)
                    return Forbid();

                if (empresaId != model.EmpresaId)
                    return Forbid();
            }

            var usuario = new Usuario(model.Nome, model.Email, model.EmpresaId, model.Cargo);

            if (!usuario.IsValid())
                return BadRequest(usuario.ValidationResult.Errors);

            var result = await _authenticationService.UserManager.CreateAsync(usuario, model.Senha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(usuario?.ToUsuarioRespostaViewModel());
        }


        [HttpPost("empresa")]
        [Authorize(Policy = "PodeCriarEmpresa")]
        public async Task<IActionResult> RegistroEmpresa(RegistroEmpresaViewModel model)
        {
            var empresa = await _mediator.Send(
                new AdicionarEmpresaCommand(model.Cnpj, model.RazaoSocial, model.NomeFantasia, model.RegimeTributario, model.Email));

            if (_notificador.TemNotificacoes())
                return BadRequest(_notificador.ObterNotificacoes());

            return Ok(empresa?.ToEmpresaRespostaViewModel());
        }

        [HttpGet("usuario/{id:int}")]
        public async Task<IActionResult> ObterUsuarioPorId(int id)
        {
            var userId = _authUser.ObterIdUsuario();

            if (userId != id)
                return Forbid();

            var usuario = await _usuarioRepository.ObterUsuarioAsync(id);

            return Ok(usuario?.ToUsuarioRespostaViewModel());
        }
    }
}
