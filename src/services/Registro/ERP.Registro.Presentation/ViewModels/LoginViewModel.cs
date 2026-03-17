using System.ComponentModel.DataAnnotations;

namespace ERP.Registro.Presentation.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}
