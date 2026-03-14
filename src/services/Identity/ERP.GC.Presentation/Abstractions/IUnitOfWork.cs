namespace ERP.GC.Presentation.Abstractions
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
