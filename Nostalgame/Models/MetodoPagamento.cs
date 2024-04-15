using Nostalgame.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class MetodoPagamento
{
    [Key]
    public int IdMetodoPagamento { get; set; }

    [Required]
    public string IdUtente { get; set; }

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } // "Carta" o "PayPal"

    // Per "Carta", questo potrebbe essere l'ultimo numero di 4 cifre della carta.
    // Per "PayPal", questo potrebbe essere l'indirizzo email dell'account PayPal.
    [Required]
    [StringLength(50)]
    public string Identificatore { get; set; }

    [ForeignKey("IdUtente")]
    public Utente Utente { get; set; }
}
