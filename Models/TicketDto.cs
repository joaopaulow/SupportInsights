namespace SupportInsights.Models
{
    public class TicketDto
    {
        public int Codigo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int CodigoCliente { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public int CodigoModulo { get; set; }
        public string NomeModulo { get; set; } = string.Empty;
    }
}