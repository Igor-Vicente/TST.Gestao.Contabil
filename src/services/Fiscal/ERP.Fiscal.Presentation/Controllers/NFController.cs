using ERP.Fiscal.Presentation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Identidade;

namespace ERP.Fiscal.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/nf")]
    public class NFController : ControllerBase
    {
        private readonly IAuthUser _authUser;
        private readonly IRegistroService _registroService;

        public NFController(IAuthUser authUser, IRegistroService registroService)
        {
            _authUser = authUser;
            _registroService = registroService;
        }


        [HttpPost("entrada")]
        [RequestSizeLimit(1 * 1024 * 1024)]
        public async Task<IActionResult> EntradaNotaFiscal(IFormFile file)
        {
            var userId = _authUser.ObterIdUsuario();

            var usuario = await _registroService.ObterUsuarioEmpresaAsync(userId);

            return Ok(usuario);
        }
    }
}
