using System.ComponentModel.DataAnnotations;

namespace Nostalgame.Models
{
    //Creo un modello per la gestione del login - contiene i campi Username, Password e RememberMe
    //lo gestisco a parte con un ViewModel per motivi di sicurezza, così che un utente malintenzionato non possa accedere o modificare i dati
    public class LoginViewModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        //rememberme è un parametro che permette di mantenere l'utente loggato, non è un campo del database

        [Display(Name = "Ricordami")]
        public bool RememberMe { get; set; }
    }
}
