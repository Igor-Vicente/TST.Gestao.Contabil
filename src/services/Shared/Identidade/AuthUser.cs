using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Shared.Identidade
{
    public interface IAuthUser
    {
        bool EstaAutenticado();
        HttpContext ObterHttpContext();
        int ObterIdUsuario();
        string ObterCargoUsuario();
        int ObterEmpresaIdUsuario();
    }

    public class AuthUser : IAuthUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AuthUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public bool EstaAutenticado()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public HttpContext ObterHttpContext()
        {
            return _accessor.HttpContext;
        }

        public int ObterIdUsuario()
        {
            return EstaAutenticado() ?
                int.Parse(_accessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value)
                : 0;
        }

        public string ObterCargoUsuario()
        {
            return EstaAutenticado() ?
                _accessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "cargo")?.Value
                : string.Empty;
        }

        public int ObterEmpresaIdUsuario()
        {
            return EstaAutenticado() ?
                int.Parse(_accessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "empresaId")?.Value)
                : 0;
        }
    }
}
