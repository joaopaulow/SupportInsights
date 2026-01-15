using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportInsights.Models;
using SupportInsights.Services;

namespace SupportInsights.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IDashboardService _dashboardService;

        public DashboardController(ILogger<DashboardController> logger, IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }
     
        [HttpGet]
        [Authorize(Roles = "Admin,Suporte")]
        [ProducesResponseType(typeof(DashboardResponsePaginado), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] int mes, 
            [FromQuery] int ano,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {                
                if (pagina < 1)
                {
                    return BadRequest(new { mensagem = "O número da página deve ser maior ou igual a 1." });
                }

                if (tamanhoPagina < 1 || tamanhoPagina > 100)
                {
                    return BadRequest(new { mensagem = "O tamanho da página deve estar entre 1 e 100." });
                }
                
                var resposta = await _dashboardService.GetDashboardAsync(mes, ano, pagina, tamanhoPagina);

                return Ok(resposta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dados do dashboard para mês {Mes} e ano {Ano}", mes, ano);
                return StatusCode(500, new { mensagem = "Ocorreu um erro ao processar a requisição.", erro = ex.Message });
            }
        }        
        
        [HttpGet("{id}", Name = "GetTicket")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetTicket(int id)
        {
            try
            {
                var ticket = await _dashboardService.GetTicketByIdAsync(id);
                
                if (ticket == null)
                    return NotFound();

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ticket com código {Codigo}", id);
                return StatusCode(500, new { mensagem = "Ocorreu um erro ao processar a requisição.", erro = ex.Message });
            }
        }
    }
}