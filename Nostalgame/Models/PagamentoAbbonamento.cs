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

        [ForeignKey("IdAbbonamento")]
        public Abbonamento Abbonamento { get; set; }

        [Required]
        public System.DateTime DataPagamento { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ImportoPagato { get; set; }

        [ForeignKey("IdUtente")]
        public Utente Utente { get; set; }
    }

}
