using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nostalgame.Models
{
    //aggiungo estensione IdentityUser per poter utilizzare Identity per il login
    public class Utente : IdentityUser
    {
        //ometto IdUtente perché Identity mi fornisce già un Id di tipo string
        [Required]
        [StringLength(20)]
        public string? Ruolo { get; set; }

        //proprietà non mappata di modo da non salvare la password nel database, è un dato temporaneo per il login
        [NotMapped]
        public string? Password { get; set; }
    }
}
