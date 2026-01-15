namespace SupportInsights.Models
{    
    public class DashboardResponsePaginado
    {
        public List<TicketDto> Tickets { get; set; } = new();
        public object AgrupadoPorCliente { get; set; } = new();
        public object AgrupadoPorModulo { get; set; } = new();
        public PaginationInfo Paginacao { get; set; } = new();
    }
}