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

        //aggiungo un campo per il costo mensile dell'abbonamento che varia tra Standard e Premium
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostoMensile { get; set; }
    }
}
