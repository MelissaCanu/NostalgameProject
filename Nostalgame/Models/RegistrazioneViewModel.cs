using System.ComponentModel.DataAnnotations;

namespace Nostalgame.Models
{
    public class RegistrazioneViewModel
    {
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} deve essere almeno {2} e al massimo {1} caratteri.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "La password deve essere lunga 8-15 caratteri e contenere almeno un numero, una lettera maiuscola, una lettera minuscola e un carattere speciale.")]
        public string Password { get; set; }

        public Registrazione Registrazione { get; set; }
    }

}
