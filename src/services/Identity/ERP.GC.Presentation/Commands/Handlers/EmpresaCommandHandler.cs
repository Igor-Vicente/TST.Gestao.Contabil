using ERP.GC.Presentation.Abstractions;
using ERP.GC.Presentation.Data.Repositories;
using ERP.GC.Presentation.Models;
using MediatR;

namespace ERP.GC.Presentation.Commands.Handlers
{
    public class EmpresaCommandHandler : IRequestHandler<AdicionarEmpresaCommand, Empresa>
    {
        private readonly IEmpresaRepository _repository;
        private readonly INotificador _notificador;

        public EmpresaCommandHandler(IEmpresaRepository repository, INotificador notificador)
        {
            _repository = repository;
            _notificador = notificador;
        }

        public async Task<Empresa> Handle(AdicionarEmpresaCommand request, CancellationToken cancellationToken)
        {
            var empresa = new Empresa(request.Cnpj, request.RazaoSocial, request.NomeFantasia, request.RegimeTributario, request.Email, request.Ativo, request.CriadoEm);

            if (!empresa.IsValid())
            {
                _notificador.AdicionarNotificacao(empresa.ValidationResult.Errors.Select(e => e.ErrorMessage));
                return null;
            }

            await _repository.AdicionarAsync(empresa);
            await _repository.UnitOfWork.CommitAsync();

            return empresa;
        }
    }
}
