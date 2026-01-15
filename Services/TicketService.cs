using Microsoft.EntityFrameworkCore;
using SupportInsights.Data;
using SupportInsights.Models;

namespace SupportInsights.Services
{   
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TicketService> _logger;

        public TicketService(ApplicationDbContext context, ILogger<TicketService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string? ErrorMessage, TicketDto? Ticket)> CriarTicketAsync(TicketCreateDto ticketDto)
        {
            if (string.IsNullOrWhiteSpace(ticketDto.Titulo))
            {
                return (false, "O título do ticket é obrigatório.", null);
            }

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Codigo == ticketDto.CodigoCliente);
            if (!clienteExiste)
            {
                return (false, $"Cliente com código {ticketDto.CodigoCliente} não encontrado.", null);
            }

            var moduloExiste = await _context.Modulos.AnyAsync(m => m.Codigo == ticketDto.CodigoModulo);
            if (!moduloExiste)
            {
                return (false, $"Módulo com código {ticketDto.CodigoModulo} não encontrado.", null);
            }

            var maxCodigo = await _context.Tickets.MaxAsync(t => (int?)t.Codigo) ?? 0;

            var ticket = new TicketDto
            {
                Codigo = maxCodigo + 1,
                Titulo = ticketDto.Titulo,
                CodigoCliente = ticketDto.CodigoCliente,
                CodigoModulo = ticketDto.CodigoModulo,
                DataAbertura = DateTime.Now,
                DataEncerramento = ticketDto.DataEncerramento
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var cliente = await _context.Clientes.FindAsync(ticket.CodigoCliente);
            var modulo = await _context.Modulos.FindAsync(ticket.CodigoModulo);

            var ticketResposta = new TicketDto
            {
                Codigo = ticket.Codigo,
                Titulo = ticket.Titulo,
                CodigoCliente = ticket.CodigoCliente,
                NomeCliente = cliente?.Nome ?? "Desconhecido",
                CodigoModulo = ticket.CodigoModulo,
                NomeModulo = modulo?.Nome ?? "Desconhecido",
                DataAbertura = ticket.DataAbertura,
                DataEncerramento = ticket.DataEncerramento
            };

            return (true, null, ticketResposta);
        }

        public async Task<(bool Success, string? ErrorMessage, TicketDto? Ticket)> AtualizarTicketAsync(int id, TicketCreateDto ticketDto)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return (false, $"Ticket com código {id} não encontrado.", null);
            }

            if (string.IsNullOrWhiteSpace(ticketDto.Titulo))
            {
                return (false, "O título do ticket é obrigatório.", null);
            }

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Codigo == ticketDto.CodigoCliente);
            if (!clienteExiste)
            {
                return (false, $"Cliente com código {ticketDto.CodigoCliente} não encontrado.", null);
            }

            var moduloExiste = await _context.Modulos.AnyAsync(m => m.Codigo == ticketDto.CodigoModulo);
            if (!moduloExiste)
            {
                return (false, $"Módulo com código {ticketDto.CodigoModulo} não encontrado.", null);
            }

            ticket.Titulo = ticketDto.Titulo;
            ticket.CodigoCliente = ticketDto.CodigoCliente;
            ticket.CodigoModulo = ticketDto.CodigoModulo;
            ticket.DataEncerramento = ticketDto.DataEncerramento;

            await _context.SaveChangesAsync();

            var cliente = await _context.Clientes.FindAsync(ticket.CodigoCliente);
            var modulo = await _context.Modulos.FindAsync(ticket.CodigoModulo);

            var ticketResposta = new TicketDto
            {
                Codigo = ticket.Codigo,
                Titulo = ticket.Titulo,
                CodigoCliente = ticket.CodigoCliente,
                NomeCliente = cliente?.Nome ?? "Desconhecido",
                CodigoModulo = ticket.CodigoModulo,
                NomeModulo = modulo?.Nome ?? "Desconhecido",
                DataAbertura = ticket.DataAbertura,
                DataEncerramento = ticket.DataEncerramento
            };

            return (true, null, ticketResposta);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeletarTicketAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return (false, $"Ticket com código {id} não encontrado.");
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}