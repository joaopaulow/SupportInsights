using SupportInsights.Models;

namespace SupportInsights.Interfaces
{   
    public interface ITicketService
    {      
        Task<(bool Success, string? ErrorMessage, TicketDto? Ticket)> CriarTicketAsync(TicketCreateDto ticketDto);        
        Task<(bool Success, string? ErrorMessage, TicketDto? Ticket)> AtualizarTicketAsync(int id, TicketCreateDto ticketDto);        
        Task<(bool Success, string? ErrorMessage)> DeletarTicketAsync(int id);
    }
}