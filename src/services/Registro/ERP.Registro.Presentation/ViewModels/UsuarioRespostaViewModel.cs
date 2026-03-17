using Shared.Models;

namespace ERP.Registro.Presentation.ViewModels
{
    public class UsuarioRespostaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public EmpresaRespostaViewModel Empresa { get; set; }
    }

    public class EmpresaRespostaViewModel
    {
        public int Id { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public RegimeTributario RegimeTributario { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
