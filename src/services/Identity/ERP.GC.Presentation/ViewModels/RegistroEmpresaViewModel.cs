using ERP.GC.Presentation.Models;
using System.ComponentModel.DataAnnotations;

namespace ERP.GC.Presentation.ViewModels
{
    public class RegistroEmpresaViewModel
    {
        [Required]
        public string Cnpj { get; set; }

        [Required]
        public string RazaoSocial { get; set; }

        [Required]
        public string NomeFantasia { get; set; }

        [Required]
        public RegimeTributario RegimeTributario { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
