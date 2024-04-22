﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nostalgame.Models
{
    public class Avatar
    {
        [Key]
        public int IdAvatar { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string? Foto { get; set; }

        public int IdGenere { get; set; }

        [ForeignKey("IdGenere")]
        public Genere Genere { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }
    }

}
