using ERP.GC.Presentation.Abstractions;
using ERP.GC.Presentation.Models;

namespace ERP.GC.Presentation.Data.Repositories
{
    public interface IEmpresaRepository : IRepository
    {
        Task AdicionarAsync(Empresa empresa);
        Task<Empresa?> FindAsync(int id);
    }
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly AuthDbContext _context;

        public EmpresaRepository(AuthDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task AdicionarAsync(Empresa empresa)
        {
            await _context.Empresas.AddAsync(empresa);
        }

        public async Task<Empresa?> FindAsync(int id)
        {
            return await _context.Empresas.FindAsync(id);
        }
    }
}
