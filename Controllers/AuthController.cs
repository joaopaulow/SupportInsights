using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportInsights.Data;
using SupportInsights.Interfaces;
using SupportInsights.Models;
using System.Security.Cryptography;
using System.Text;

namespace SupportInsights.Controllers
{   
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(
            ILogger<AuthController> logger, 
            ApplicationDbContext context,
            ITokenService tokenService)
        {
            _logger = logger;
            _context = context;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT
        /// </summary>
        /// <param name="loginRequest">Credenciais de login</param>
        /// <returns>Token JWT e informações do usuário</returns>
        /// <response code="200">Autenticação bem-sucedida</response>
        /// <response code="400">Credenciais inválidas</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST /auth/login
        ///     {
        ///        "email": "admin@supportinsights.com",
        ///        "senha": "Admin@123"
        ///     }
        ///     
        /// Usuários disponíveis:
        /// - admin@supportinsights.com / Admin@123 (Role: Admin)
        /// - suporte@supportinsights.com / Suporte@123 (Role: Suporte)
        /// - cliente@email.com / Cliente@123 (Role: Cliente)
        /// 
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Senha))
                {
                    return BadRequest(new { mensagem = "Email e senha são obrigatórios." });
                }

                var senhaHash = GerarHashSenha(loginRequest.Senha);
                
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.Senha == senhaHash);

                if (usuario == null)
                {
                    return BadRequest(new { mensagem = "Email ou senha inválidos." });
                }

                var token = _tokenService.GerarToken(usuario.Email, usuario.Role, usuario.Nome);

                var response = new LoginResponse
                {
                    Token = token,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Role = usuario.Role,
                    Expiracao = DateTime.UtcNow.AddHours(8)
                };

                _logger.LogInformation("Usuário {Email} autenticado com sucesso", usuario.Email);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário");
                return StatusCode(500, new { mensagem = "Ocorreu um erro ao processar a requisição.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém informações do usuário autenticado
        /// </summary>
        /// <returns>Informações do usuário</returns>
        /// <response code="200">Informações do usuário</response>
        /// <response code="401">Não autenticado</response>
        [HttpGet("getUser")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var nome = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                email,
                nome,
                role,
                autenticado = true
            });
        }

        private string GerarHashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}