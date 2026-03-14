using System.ComponentModel.DataAnnotations;

namespace ERP.GC.Presentation.ViewModels
{
    public class RefreshTokenViewModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
