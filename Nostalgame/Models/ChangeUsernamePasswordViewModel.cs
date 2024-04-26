using System.ComponentModel.DataAnnotations;

namespace Nostalgame.Models
{
    public class ChangeUsernamePasswordViewModel
    {
        public int IdRegistrazione { get; set; }


        [Required]
        [Display(Name = "Username corrente")]
        public string CurrentUsername { get; set; }

        [Required]
        [Display(Name = "Nuovo username")]
        public string NewUsername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password corrente")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} deve essere almeno {2} e al massimo {1} caratteri.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma nuova password")]
        [Compare("NewPassword", ErrorMessage = "La nuova password e la password di conferma non corrispondono.")]
        public string ConfirmPassword { get; set; }

        

    }
}
