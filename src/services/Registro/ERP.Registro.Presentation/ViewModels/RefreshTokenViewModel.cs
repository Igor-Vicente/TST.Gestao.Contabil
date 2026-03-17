using System.ComponentModel.DataAnnotations;

namespace ERP.Registro.Presentation.ViewModels
{
    public class RefreshTokenViewModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
