using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nostalgame.Models
{
    //questa tabella contiene le informazioni relative ai pagamenti degli abbonamenti e si collega alle tabelle Utenti e Abbonamenti
    public class PagamentoAbbonamento
    {
        [Key]
        public int IdPagamentoAbbonamento { get; set; }

        [Required]
        public string IdUtente { get; set; }

        [Required]
        public int IdAbbonamento { get; set; }

        [Required]
        public System.DateTime DataPagamento { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ImportoPagato { get; set; }

        public int? IdMetodoPagamento { get; set; }

        [ForeignKey("IdMetodoPagamento")]
        public MetodoPagamento MetodoPagamento { get; set; }

        //setting the foreign key for the user - this is a navigation property which is used to load the related entity
        [ForeignKey("IdUtente")]
        public Utente Utente { get; set; }
    }
}
