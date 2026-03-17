using ERP.Registro.Presentation.Models;
using ERP.Registro.Presentation.ViewModels;

namespace ERP.Registro.Presentation.Configuration
{
    public static class Mapper
    {
        public static UsuarioRespostaViewModel ToUsuarioRespostaViewModel(this Usuario usuario)
        {
            return new UsuarioRespostaViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Ativo = usuario.Ativo,
                Cargo = usuario.Cargo.ToString(),
                CriadoEm = usuario.CriadoEm,
                Empresa = usuario.Empresa?.ToEmpresaRespostaViewModel()
            };
        }

        public static EmpresaRespostaViewModel ToEmpresaRespostaViewModel(this Empresa empresa)
        {
            return new EmpresaRespostaViewModel
            {
                Id = empresa.Id,
                Cnpj = empresa.Cnpj,
                RazaoSocial = empresa.RazaoSocial,
                NomeFantasia = empresa.NomeFantasia,
                RegimeTributario = empresa.RegimeTributario,
                Email = empresa.Email,
                Ativo = empresa.Ativo,
                CriadoEm = empresa.CriadoEm
            };
        }
    }
}
