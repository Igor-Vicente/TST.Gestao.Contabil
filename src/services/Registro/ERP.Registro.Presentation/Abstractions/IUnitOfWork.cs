namespace ERP.Registro.Presentation.Abstractions
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
