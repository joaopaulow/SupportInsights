namespace SupportInsights.Models
{
    public class TicketCreateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public int CodigoCliente { get; set; }
        public int CodigoModulo { get; set; }
        public DateTime? DataEncerramento { get; set; }
    }
}