using Nostalgame.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Noleggio
{
    [Key]
    [Display(Name = "ID Noleggio")]
    public int IdNoleggio { get; set; }

    [Required]
    [Display(Name = "ID Videogioco")]
    [ForeignKey("Videogioco")]
    public int IdVideogioco { get; set; }

    [Required]
    [Display(Name = "Utente Noleggiante")]
    public string IdUtenteNoleggiante { get; set; }

    [Required]
    [Display(Name = "Data inizio")]
    public DateTime DataInizio { get; set; }

    [Required]
    [Display(Name = "Data fine")]
    public DateTime DataFine { get; set; }

    [Required]
    [Display(Name = "Indirizzo di consegna")]
    [StringLength(200)]
    public string IndirizzoSpedizione { get; set; }

    [Required]
    [Display(Name = "Prezzo")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal CostoNoleggio { get; set; }

    [Required]
    [Display(Name = "Spese di spedizione")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal SpeseSpedizione { get; set; } // Nuovo campo per le spese di spedizione

    [Display(Name = "ID Pagamento Stripe")]
    public string? StripePaymentId { get; set; }

    public Videogioco Videogioco { get; set; }
}
