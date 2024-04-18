using Nostalgame.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nostalgame.Models
{
    //questa tabella contiene le informazioni relative a un singolo noleggio ed è collegabile alle tabelle Utenti e Videogiochi
    public class Noleggio
    {
        [Key]
        public int IdNoleggio { get; set; }
        public int? IdVideogioco { get; set; }
        [Required]
        public System.DateTime DataInizio { get; set; }

        //datetime nullable per permettere di restituire il videogioco
        public System.DateTime? DataFine { get; set; }
        [Required]
        public bool Restituito { get; set; }

        //aggiungo un campo per il costo della spedizione che varia in base al tipo di abbonamento
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostoSpedizione { get; set; }

        // ID dell'utente che ha noleggiato il videogioco
        public string NoleggianteId { get; set; }


        // Navigazione alla proprietà Utente per ottenere le informazioni dell'utente noleggiante
        [ForeignKey("NoleggianteId")]
        public Utente Noleggiante { get; set; }

    }
}
