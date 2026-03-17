using ERP.Registro.Presentation.Abstractions;

namespace ERP.Registro.Presentation.Data.Repositories
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }

}
