using ERP.GC.Presentation.Abstractions;
using ERP.GC.Presentation.Models;

namespace ERP.GC.Presentation.Data.Repositories
{
    public interface IUsuarioRepository : IRepository
    {
        Task AdicionarAsync(Usuario Usuario);
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AuthDbContext _context;

        public UsuarioRepository(AuthDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task AdicionarAsync(Usuario Usuario)
        {
            await _context.Usuarios.AddAsync(Usuario);
        }
    }
}
