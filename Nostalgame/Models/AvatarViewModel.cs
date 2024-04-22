using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Nostalgame.Models
{
    public class AvatarViewModel
    {
        public int IdAvatar { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string? Foto { get; set; }

        public int IdGenere { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }
    }
}
