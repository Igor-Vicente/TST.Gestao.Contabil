using ERP.GC.Presentation.Abstractions;

namespace ERP.GC.Presentation.Data.Repositories
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }

}
