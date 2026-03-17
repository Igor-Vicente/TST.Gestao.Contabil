using ERP.Registro.Presentation.Abstractions;
using ERP.Registro.Presentation.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Registro.Presentation.Data.Repositories
{
    public interface IUsuarioRepository : IRepository
    {
        Task<Usuario?> ObterUsuarioAsync(int id);
    }
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AuthDbContext _context;

        public UsuarioRepository(AuthDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Usuario?> ObterUsuarioAsync(int id)
        {
            return await _context.Users.Include(u => u.Empresa).FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
