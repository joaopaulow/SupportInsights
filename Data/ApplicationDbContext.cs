using Microsoft.EntityFrameworkCore;
using SupportInsights.Models;
using System.Security.Cryptography;
using System.Text;

namespace SupportInsights.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<TicketDto> Tickets { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Codigo);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            });
            
            modelBuilder.Entity<Modulo>(entity =>
            {
                entity.HasKey(e => e.Codigo);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            });
            
            modelBuilder.Entity<TicketDto>(entity =>
            {
                entity.HasKey(e => e.Codigo);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CodigoCliente).IsRequired();
                entity.Property(e => e.CodigoModulo).IsRequired();
                entity.Property(e => e.DataAbertura).IsRequired();
                entity.Ignore(e => e.NomeCliente);
                entity.Ignore(e => e.NomeModulo);
            });
            
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Senha).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            });
            
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente { Codigo = 1, Nome = "Cliente A" },
                new Cliente { Codigo = 2, Nome = "Cliente B" },
                new Cliente { Codigo = 3, Nome = "Cliente C" },
                new Cliente { Codigo = 4, Nome = "Cliente D" },
                new Cliente { Codigo = 5, Nome = "Cliente E" },
                new Cliente { Codigo = 6, Nome = "Cliente F" },
                new Cliente { Codigo = 7, Nome = "Cliente G" },
                new Cliente { Codigo = 8, Nome = "Cliente H" }
            );
            
            modelBuilder.Entity<Modulo>().HasData(
                new Modulo { Codigo = 1, Nome = "Financeiro" },
                new Modulo { Codigo = 2, Nome = "RH" },
                new Modulo { Codigo = 3, Nome = "Vendas" },
                new Modulo { Codigo = 4, Nome = "Estoque" },
                new Modulo { Codigo = 5, Nome = "Compras" },
                new Modulo { Codigo = 6, Nome = "Produção" },
                new Modulo { Codigo = 7, Nome = "CRM" }
            );
            
            modelBuilder.Entity<TicketDto>().HasData(
                new TicketDto { Codigo = 1, Titulo = "Erro no relatório", CodigoCliente = 1, CodigoModulo = 1, DataAbertura = new DateTime(2026, 1, 10), DataEncerramento = new DateTime(2026, 1, 15) },
                new TicketDto { Codigo = 2, Titulo = "Problema de acesso", CodigoCliente = 2, CodigoModulo = 2, DataAbertura = new DateTime(2026, 1, 12), DataEncerramento = null },
                new TicketDto { Codigo = 3, Titulo = "Dúvida sobre lançamento", CodigoCliente = 1, CodigoModulo = 1, DataAbertura = new DateTime(2026, 1, 13), DataEncerramento = new DateTime(2026, 1, 14) },
                new TicketDto { Codigo = 4, Titulo = "Sistema lento ao processar", CodigoCliente = 3, CodigoModulo = 4, DataAbertura = new DateTime(2026, 1, 15), DataEncerramento = new DateTime(2026, 1, 20) },
                new TicketDto { Codigo = 5, Titulo = "Erro ao gerar nota fiscal", CodigoCliente = 4, CodigoModulo = 3, DataAbertura = new DateTime(2026, 1, 18), DataEncerramento = null },
                new TicketDto { Codigo = 6, Titulo = "Inconsistência no estoque", CodigoCliente = 5, CodigoModulo = 4, DataAbertura = new DateTime(2026, 1, 22), DataEncerramento = new DateTime(2026, 1, 25) },
                new TicketDto { Codigo = 7, Titulo = "Folha de pagamento incorreta", CodigoCliente = 2, CodigoModulo = 2, DataAbertura = new DateTime(2026, 1, 25), DataEncerramento = null },
                new TicketDto { Codigo = 8, Titulo = "Dashboard não carrega", CodigoCliente = 6, CodigoModulo = 7, DataAbertura = new DateTime(2026, 1, 28), DataEncerramento = new DateTime(2026, 1, 30) },
                new TicketDto { Codigo = 9, Titulo = "Erro ao importar XML", CodigoCliente = 1, CodigoModulo = 5, DataAbertura = new DateTime(2026, 2, 3), DataEncerramento = new DateTime(2026, 2, 5) },
                new TicketDto { Codigo = 10, Titulo = "Problema no cadastro de produtos", CodigoCliente = 4, CodigoModulo = 4, DataAbertura = new DateTime(2026, 2, 7), DataEncerramento = null },
                new TicketDto { Codigo = 11, Titulo = "Integração com API falhou", CodigoCliente = 7, CodigoModulo = 7, DataAbertura = new DateTime(2026, 2, 10), DataEncerramento = new DateTime(2026, 2, 12) },
                new TicketDto { Codigo = 12, Titulo = "Relatório de vendas zerado", CodigoCliente = 3, CodigoModulo = 3, DataAbertura = new DateTime(2026, 2, 14), DataEncerramento = null },
                new TicketDto { Codigo = 13, Titulo = "Ordem de produção duplicada", CodigoCliente = 8, CodigoModulo = 6, DataAbertura = new DateTime(2026, 2, 18), DataEncerramento = new DateTime(2026, 2, 20) },
                new TicketDto { Codigo = 14, Titulo = "Erro ao calcular impostos", CodigoCliente = 2, CodigoModulo = 1, DataAbertura = new DateTime(2026, 2, 22), DataEncerramento = null },
                new TicketDto { Codigo = 15, Titulo = "Bug no sistema", CodigoCliente = 1, CodigoModulo = 3, DataAbertura = new DateTime(2026, 12, 5), DataEncerramento = null },
                new TicketDto { Codigo = 16, Titulo = "Lentidão no carregamento", CodigoCliente = 3, CodigoModulo = 1, DataAbertura = new DateTime(2026, 12, 15), DataEncerramento = new DateTime(2026, 12, 18) },
                new TicketDto { Codigo = 17, Titulo = "Erro ao exportar relatório", CodigoCliente = 2, CodigoModulo = 2, DataAbertura = new DateTime(2026, 12, 20), DataEncerramento = null },
                new TicketDto { Codigo = 18, Titulo = "Falha no backup automático", CodigoCliente = 5, CodigoModulo = 1, DataAbertura = new DateTime(2026, 12, 8), DataEncerramento = new DateTime(2026, 12, 10) },
                new TicketDto { Codigo = 19, Titulo = "Problema na impressão de boletos", CodigoCliente = 6, CodigoModulo = 1, DataAbertura = new DateTime(2026, 12, 12), DataEncerramento = null },
                new TicketDto { Codigo = 20, Titulo = "Erro ao fechar pedido", CodigoCliente = 4, CodigoModulo = 3, DataAbertura = new DateTime(2026, 12, 18), DataEncerramento = new DateTime(2026, 12, 22) },
                new TicketDto { Codigo = 21, Titulo = "Sistema travando ao abrir tela", CodigoCliente = 7, CodigoModulo = 5, DataAbertura = new DateTime(2026, 12, 23), DataEncerramento = null },
                new TicketDto { Codigo = 22, Titulo = "Inconsistência em contas a pagar", CodigoCliente = 8, CodigoModulo = 1, DataAbertura = new DateTime(2026, 12, 27), DataEncerramento = new DateTime(2026, 12, 30) }
            );
                  
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario 
                { 
                    Id = 1, 
                    Nome = "Administrador", 
                    Email = "admin@supportinsights.com", 
                    Senha = GerarHashSenha("Admin@123"),
                    Role = "Admin" 
                },
                new Usuario 
                { 
                    Id = 2, 
                    Nome = "Equipe Suporte", 
                    Email = "suporte@supportinsights.com", 
                    Senha = GerarHashSenha("Suporte@123"),
                    Role = "Suporte" 
                },
                new Usuario 
                { 
                    Id = 3, 
                    Nome = "Cliente Teste", 
                    Email = "cliente@email.com", 
                    Senha = GerarHashSenha("Cliente@123"),
                    Role = "Cliente" 
                }
            );
        }

        private static string GerarHashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}