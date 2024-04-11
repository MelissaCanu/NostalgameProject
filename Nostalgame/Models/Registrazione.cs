using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nostalgame.Models
{
    public class Registrazione
    {
        [Key]
        public int IdRegistrazione { get; set; }

        public string IdUtente { get; set; }

        public int IdAbbonamento { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        public string Cognome { get; set; }

        [Required]
        [StringLength(200)]
        public string Indirizzo { get; set; }

        [Required]
        [StringLength(50)]
        public string Citta { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [ForeignKey("IdUtente")]
        public Utente Utente { get; set; }

        [ForeignKey("IdAbbonamento")]
        public Abbonamento Abbonamento { get; set; }
    }

}
