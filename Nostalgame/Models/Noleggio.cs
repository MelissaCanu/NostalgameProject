using Nostalgame.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Noleggio
{
    [Key]
    public int IdNoleggio { get; set; }

    [Required]
    [ForeignKey("Videogioco")]
    public int IdVideogioco { get; set; }

    [Required]
    public string IdUtenteNoleggiante { get; set; }

    [Required]
    public DateTime DataInizio { get; set; }

    [Required]
    public DateTime DataFine { get; set; }

    [Required]
    [StringLength(200)]
    public string IndirizzoSpedizione { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal CostoNoleggio { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal SpeseSpedizione { get; set; } // Nuovo campo per le spese di spedizione

    public string? StripePaymentId { get; set; }

    public Videogioco Videogioco { get; set; }
}
