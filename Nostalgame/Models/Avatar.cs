using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nostalgame.Models
{
    public class Avatar
    {
        [Key]
        public int IdAvatar { get; set; }

        [Required]
        public string IdUtente { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }
        [Required]
        [StringLength(500)]
        public string Foto { get; set; }

        //setting the foreign key for the user - this is a navigation property which is used to load the related entity
        [ForeignKey("IdUtente")]
        public Utente Utente { get; set; }

        //creo una proprietà che non è presente nel database per poter caricare l'immagine
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
