namespace Nostalgame.Models
{
    public class AbbonamentoViewModel
    {
        public int IdPagamentoAbbonamento { get; set; }
        public string IdUtente { get; set; }
        public int IdAbbonamento { get; set; }
        public DateTime DataPagamento { get; set; }
        public decimal CostoMensile { get; set; }
        public decimal ImportoPagato { get; set; }
    }
}
