using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nostalgame.Models
{
    //questa tabella contiene le informazioni relative al genere di un videogioco e si collega alla tabella Videogiochi
    //la creo separatamente per rendere più semplice aggiornare i generi, eseguire ricerche filtrate in base al genere e per rendere più semplice la gestione dei dati
    public class Genere
    {
        [Key]
        public int IdGenere { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }
    }

}
