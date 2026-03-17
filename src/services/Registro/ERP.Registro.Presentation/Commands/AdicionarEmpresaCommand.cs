using ERP.Registro.Presentation.Abstractions;
using ERP.Registro.Presentation.Models;
using Shared.Models;

namespace ERP.Registro.Presentation.Commands
{
    public class AdicionarEmpresaCommand : Command<Empresa>
    {
        public string Cnpj { get; private set; }
        public string RazaoSocial { get; private set; }
        public string NomeFantasia { get; private set; }
        public RegimeTributario RegimeTributario { get; private set; }
        public string Email { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }

        public AdicionarEmpresaCommand(string cnpj, string razaoSocial, string nomeFantasia, RegimeTributario regimeTributario, string email)
        {
            Cnpj = cnpj;
            RazaoSocial = razaoSocial;
            NomeFantasia = nomeFantasia;
            RegimeTributario = regimeTributario;
            Email = email;
            Ativo = true;
            CriadoEm = DateTime.UtcNow;
        }
    }
}
