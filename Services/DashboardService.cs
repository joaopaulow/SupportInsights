using Microsoft.EntityFrameworkCore;
using SupportInsights.Data;
using SupportInsights.Models;

namespace SupportInsights.Services
{   
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(ApplicationDbContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardResponsePaginado> GetDashboardAsync(int mes, int ano, int pagina, int tamanhoPagina)
        {
            var todosTickets = await _context.Tickets
                .Where(t => t.DataAbertura.Month == mes && t.DataAbertura.Year == ano)
                .ToListAsync();

            var totalItens = todosTickets.Count;
            var totalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina);

            var ticketsPaginados = todosTickets
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(t => new TicketDto
                {
                    Codigo = t.Codigo,
                    Titulo = t.Titulo,
                    CodigoCliente = t.CodigoCliente,
                    NomeCliente = _context.Clientes.FirstOrDefault(c => c.Codigo == t.CodigoCliente)?.Nome ?? "Desconhecido",
                    DataAbertura = t.DataAbertura,
                    DataEncerramento = t.DataEncerramento,
                    CodigoModulo = t.CodigoModulo,
                    NomeModulo = _context.Modulos.FirstOrDefault(m => m.Codigo == t.CodigoModulo)?.Nome ?? "Desconhecido"
                })
                .ToList();

            var todosTicketsDto = todosTickets
                .Select(t => new TicketDto
                {
                    Codigo = t.Codigo,
                    Titulo = t.Titulo,
                    CodigoCliente = t.CodigoCliente,
                    NomeCliente = _context.Clientes.FirstOrDefault(c => c.Codigo == t.CodigoCliente)?.Nome ?? "Desconhecido",
                    DataAbertura = t.DataAbertura,
                    DataEncerramento = t.DataEncerramento,
                    CodigoModulo = t.CodigoModulo,
                    NomeModulo = _context.Modulos.FirstOrDefault(m => m.Codigo == t.CodigoModulo)?.Nome ?? "Desconhecido"
                })
                .ToList();

            var agrupadoPorCliente = todosTicketsDto
                .GroupBy(t => t.CodigoCliente)
                .Select(g => new
                {
                    CodigoCliente = g.Key,
                    NomeCliente = g.First().NomeCliente,
                    Quantidade = g.Count()
                })
                .OrderByDescending(x => x.Quantidade)
                .ToList();

            var agrupadoPorModulo = todosTicketsDto
                .GroupBy(t => t.CodigoModulo)
                .Select(g => new
                {
                    CodigoModulo = g.Key,
                    NomeModulo = g.First().NomeModulo,
                    Quantidade = g.Count()
                })
                .OrderByDescending(x => x.Quantidade)
                .ToList();

            var paginacaoInfo = new PaginationInfo
            {
                PaginaAtual = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalItens = totalItens,
                TotalPaginas = totalPaginas,
                TemPaginaAnterior = pagina > 1,
                TemProximaPagina = pagina < totalPaginas
            };

            return new DashboardResponsePaginado
            {
                Tickets = ticketsPaginados,
                AgrupadoPorCliente = agrupadoPorCliente,
                AgrupadoPorModulo = agrupadoPorModulo,
                Paginacao = paginacaoInfo
            };
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
                return null;

            var cliente = await _context.Clientes.FindAsync(ticket.CodigoCliente);
            var modulo = await _context.Modulos.FindAsync(ticket.CodigoModulo);

            return new TicketDto
            {
                Codigo = ticket.Codigo,
                Titulo = ticket.Titulo,
                CodigoCliente = ticket.CodigoCliente,
                NomeCliente = cliente?.Nome ?? "Desconhecido",
                DataAbertura = ticket.DataAbertura,
                DataEncerramento = ticket.DataEncerramento,
                CodigoModulo = ticket.CodigoModulo,
                NomeModulo = modulo?.Nome ?? "Desconhecido"
            };
        }
    }
}