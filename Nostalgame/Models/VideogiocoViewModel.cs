using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Nostalgame.Models
{
    public class VideogiocoViewModel
    {
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

        [StringLength(500)]
        public string? Foto { get; set; }

        [Required]
        public int Anno { get; set; }

        [Required]
        public bool Disponibile { get; set; }

        [Required]
        public int IdGenere { get; set; }

        [Required]
        public string IdProprietario { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }
    }
}
