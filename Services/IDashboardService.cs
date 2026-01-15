using SupportInsights.Models;

namespace SupportInsights.Services
{   
    public interface IDashboardService
    {        
        Task<DashboardResponsePaginado> GetDashboardAsync(int mes, int ano, int pagina, int tamanhoPagina);
       
        Task<TicketDto?> GetTicketByIdAsync(int id);
    }
}