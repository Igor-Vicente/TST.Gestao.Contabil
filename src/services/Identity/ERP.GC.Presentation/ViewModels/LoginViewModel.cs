using System.ComponentModel.DataAnnotations;

namespace ERP.GC.Presentation.ViewModels
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
