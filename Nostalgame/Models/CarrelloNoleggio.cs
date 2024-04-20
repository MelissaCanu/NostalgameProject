using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nostalgame.Models
{
    public class CarrelloNoleggio
    {
        [Key]
        public int Id { get; set; }

        public string UtenteId { get; set; }

        // Navigazione alla proprietà Utente
        [ForeignKey("UtenteId")]
        public virtual Utente Utente { get; set; }


    }

}
