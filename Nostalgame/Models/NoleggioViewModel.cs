using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class NoleggioViewModel
{
    public int IdNoleggio { get; set; }

    [Required]
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

    public string? StripePaymentId { get; set; }
}
