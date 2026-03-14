using ERP.GC.Presentation.Abstractions;
using ERP.GC.Presentation.Models;

namespace ERP.GC.Presentation.Commands
{
    public class AdicionarUsuarioCommand : Command<Usuario>
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public int EmpresaId { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }

        public AdicionarUsuarioCommand(int id, string nome, string email, int empresaId)
        {
            Id = id;
            Nome = nome;
            Email = email;
            EmpresaId = empresaId;
            Ativo = true;
            CriadoEm = DateTime.UtcNow;
        }
    }
}
