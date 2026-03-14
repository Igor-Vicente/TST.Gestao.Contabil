using System.ComponentModel.DataAnnotations;

namespace ERP.GC.Presentation.ViewModels
{
    public class RegistroUsuarioViewModel
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int EmpresaId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        [Required]
        [Compare("Senha")]
        public string SenhaConfirmacao { get; set; }
    }
}
