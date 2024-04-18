using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nostalgame.Models
{
    public class Abbonamento
    {
        [Key]
        public int IdAbbonamento { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoAbbonamento { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostoMensile { get; set; } 
    }

}
