using ERP.GC.Presentation.Abstractions;
using ERP.GC.Presentation.Data.Repositories;
using ERP.GC.Presentation.Models;
using MediatR;

namespace ERP.GC.Presentation.Commands.Handlers
{
    public sealed class UsuarioCommandHandler : IRequestHandler<AdicionarUsuarioCommand, Usuario>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly INotificador _notificador;

        public UsuarioCommandHandler(INotificador notificador, IUsuarioRepository usuarioRepository, IEmpresaRepository empresaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _empresaRepository = empresaRepository;
            _notificador = notificador;
        }

        public async Task<Usuario> Handle(AdicionarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = new Usuario(request.Id, request.Nome, request.Email, request.EmpresaId, request.Ativo, request.CriadoEm);

            if (!usuario.IsValid())
            {
                _notificador.AdicionarNotificacao(usuario.ValidationResult.Errors.Select(e => e.ErrorMessage));
                return null;
            }

            var empresa = await _empresaRepository.FindAsync(request.EmpresaId);

            if (empresa == null)
            {
                _notificador.AdicionarNotificacao("Empresa não cadastrada.");
                return null;
            }

            await _usuarioRepository.AdicionarAsync(usuario);
            await _usuarioRepository.UnitOfWork.CommitAsync();

            return usuario;
        }
    }
}
