using Nostalgame.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nostalgame.Models
{
    //questa tabella contiene le informazioni relative ai videogiochi e si collega alla tabella Genere
    public class Videogioco
    {
        [Key]
        public int IdVideogioco { get; set; }

        [Required]
        [StringLength(50)]
        public string Titolo { get; set; }

        [Required]
        [StringLength(50)]
        public string Piattaforma { get; set; }

        [Required]
        [StringLength(50)]
        public string CasaProduttrice { get; set; }

        [Required]
        [StringLength(500)]
        public string Descrizione { get; set; }
        [Required]
        [StringLength(500)]
        public string Foto { get; set; }

        [Required]
        public int Anno { get; set; }

        [Required]
        public bool Disponibile { get; set; }

        [Required]
        public int IdGenere { get; set; }

        // ID dell'utente che possiede il videogioco
        //trasformo in string per poter collegare la tabella Videogioco con la tabella Utente
        [Required]
        public string IdProprietario { get; set; }

        // Navigazione alla proprietà Utente per ottenere le informazioni dell'utente proprietario
        [ForeignKey("IdProprietario")]
        public Utente Proprietario { get; set; }

        [ForeignKey("IdGenere")]
        public Genere Genere { get; set; }

        //creo una proprietà che non è presente nel database per poter caricare l'immagine
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
