using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportInsights.Interfaces;
using SupportInsights.Models;

namespace SupportInsights.Controllers
{    
    [ApiController]
    [Route("tickets")]
    [Produces("application/json")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        private readonly ITicketService _ticketService;

        public TicketController(ILogger<TicketController> logger, ITicketService ticketService)
        {
            _logger = logger;
            _ticketService = ticketService;
        }
       
        [HttpPost]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TicketDto>> CriarTicket([FromBody] TicketCreateDto ticketDto)
        {
            try
            {
                var (success, errorMessage, ticket) = await _ticketService.CriarTicketAsync(ticketDto);
                
                if (!success)
                {
                    return BadRequest(new { mensagem = errorMessage });
                }
                
                return CreatedAtRoute("GetTicket", new { id = ticket!.Codigo }, ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar ticket");
                return StatusCode(500, new { mensagem = "Ocorreu um erro ao processar a requisição.", erro = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TicketDto>> AtualizarTicket(int id, [FromBody] TicketCreateDto ticketDto)
        {
            try
            {
                var (success, errorMessage, ticket) = await _ticketService.AtualizarTicketAsync(id, ticketDto);
                
                if (!success)
                {
                    if (errorMessage!.StartsWith("Ticket com código"))
                    {
                        return NotFound(new { mensagem = errorMessage });
                    }
                    return BadRequest(new { mensagem = errorMessage });
                }
                
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar ticket");
                return StatusCode(500, new { mensagem = "Ocorreu um erro ao processar a requisição.", erro = ex.Message });
            }
        }
       
        [HttpDelete("{id}")]        
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletarTicket(int id)
        {
            try
            {
                var (success, errorMessage) = await _ticketService.DeletarTicketAsync(id);
                
                if (!success)
                {
                    return NotFound(new { mensagem = errorMessage });
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar ticket");
                return StatusCode(500, new { mensagem = "Ocorreu um erro ao processar a requisição.", erro = ex.Message });
            }
        }
    }
}