using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nostalgame.Models
{
    //aggiungo estensione IdentityUser per poter utilizzare Identity per il login
    public class Utente : IdentityUser
    {
        [Required]
        [StringLength(20)]
        public string? Ruolo { get; set; } = "User";

        public int? IdAvatar { get; set; }

        [ForeignKey("IdAvatar")]
        public Avatar Avatar { get; set; }
        public string StripeCustomerId { get; set; }

        // Un utente può possedere molti videogiochi
        public virtual ICollection<Videogioco> Videogiochi { get; set; }



    }

}
