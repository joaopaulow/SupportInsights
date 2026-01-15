using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SupportInsights.Controllers;
using SupportInsights.Data;
using SupportInsights.Models;
using SupportInsights.Services;
using Xunit;

namespace SupportInsights.Tests
{
    public class TicketServiceTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        private TicketController GetController(ITicketService ticketService)
        {
            var loggerMock = new Mock<ILogger<TicketController>>();
            return new TicketController(loggerMock.Object, ticketService);
        }

        [Fact]
        public async Task CriarTicket_RetornaBadRequest_QuandoTituloVazio()
        {
            var context = GetDbContext(nameof(CriarTicket_RetornaBadRequest_QuandoTituloVazio));
            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "",
                CodigoCliente = 1,
                CodigoModulo = 1
            };

            var result = await controller.CriarTicket(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("título", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CriarTicket_RetornaBadRequest_QuandoTituloNulo()
        {
            var context = GetDbContext(nameof(CriarTicket_RetornaBadRequest_QuandoTituloNulo));
            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = null!,
                CodigoCliente = 1,
                CodigoModulo = 1
            };

            var result = await controller.CriarTicket(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("título", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CriarTicket_RetornaBadRequest_QuandoClienteNaoExiste()
        {
            var context = GetDbContext(nameof(CriarTicket_RetornaBadRequest_QuandoClienteNaoExiste));
            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Teste",
                CodigoCliente = 999,
                CodigoModulo = 1
            };

            var result = await controller.CriarTicket(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Cliente", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CriarTicket_RetornaBadRequest_QuandoModuloNaoExiste()
        {
            var context = GetDbContext(nameof(CriarTicket_RetornaBadRequest_QuandoModuloNaoExiste));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Teste",
                CodigoCliente = 1,
                CodigoModulo = 999
            };

            var result = await controller.CriarTicket(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Módulo", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CriarTicket_RetornaCreated_QuandoDadosValidos()
        {
            var context = GetDbContext(nameof(CriarTicket_RetornaCreated_QuandoDadosValidos));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Teste",
                CodigoCliente = 1,
                CodigoModulo = 1
            };

            var result = await controller.CriarTicket(dto);

            var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.NotNull(created.Value);
            var ticket = created.Value as TicketDto;
            Assert.NotNull(ticket);
            Assert.Equal("Teste", ticket.Titulo);
            Assert.Equal(1, ticket.CodigoCliente);
            Assert.Equal("Cliente 1", ticket.NomeCliente);
        }

        [Fact]
        public async Task CriarTicket_GeraNovoCodigo_Sequencialmente()
        {
            var context = GetDbContext(nameof(CriarTicket_GeraNovoCodigo_Sequencialmente));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket Existente",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Novo Ticket",
                CodigoCliente = 1,
                CodigoModulo = 1
            };

            var result = await controller.CriarTicket(dto);

            var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
            var ticket = created.Value as TicketDto;
            Assert.NotNull(ticket);
            Assert.Equal(2, ticket.Codigo);
        }

        [Fact]
        public async Task CriarTicket_DefineDataAbertura_Automaticamente()
        {
            var context = GetDbContext(nameof(CriarTicket_DefineDataAbertura_Automaticamente));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Teste",
                CodigoCliente = 1,
                CodigoModulo = 1
            };

            var result = await controller.CriarTicket(dto);

            var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
            var ticket = created.Value as TicketDto;
            Assert.NotNull(ticket);
            Assert.True((DateTime.Now - ticket.DataAbertura).TotalSeconds < 5);
        }

        // ============ TESTES DE UPDATE ============

        [Fact]
        public async Task AtualizarTicket_RetornaNotFound_QuandoTicketNaoExiste()
        {
            var context = GetDbContext(nameof(AtualizarTicket_RetornaNotFound_QuandoTicketNaoExiste));
            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Ticket Atualizado",
                CodigoCliente = 1,
                CodigoModulo = 1
            };

            var result = await controller.AtualizarTicket(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("999", notFound.Value.ToString());
        }

        [Fact]
        public async Task AtualizarTicket_RetornaBadRequest_QuandoTituloVazio()
        {
            var context = GetDbContext(nameof(AtualizarTicket_RetornaBadRequest_QuandoTituloVazio));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket Original",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "",
                CodigoCliente = 1,
                CodigoModulo = 1
            };

            var result = await controller.AtualizarTicket(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("título", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AtualizarTicket_RetornaBadRequest_QuandoClienteNaoExiste()
        {
            var context = GetDbContext(nameof(AtualizarTicket_RetornaBadRequest_QuandoClienteNaoExiste));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket Original",
                CodigoCliente = 2,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Ticket Atualizado",
                CodigoCliente = 999,
                CodigoModulo = 1
            };

            var result = await controller.AtualizarTicket(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Cliente", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AtualizarTicket_RetornaBadRequest_QuandoModuloNaoExiste()
        {
            var context = GetDbContext(nameof(AtualizarTicket_RetornaBadRequest_QuandoModuloNaoExiste));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket Original",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Ticket Atualizado",
                CodigoCliente = 1,
                CodigoModulo = 999
            };

            var result = await controller.AtualizarTicket(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Módulo", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AtualizarTicket_RetornaOk_QuandoDadosValidos()
        {
            var context = GetDbContext(nameof(AtualizarTicket_RetornaOk_QuandoDadosValidos));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Clientes.Add(new Cliente { Codigo = 2, Nome = "Cliente 2" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Modulos.Add(new Modulo { Codigo = 2, Nome = "Modulo 2" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket Original",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Ticket Atualizado",
                CodigoCliente = 2,
                CodigoModulo = 2,
                DataEncerramento = DateTime.Now
            };

            var result = await controller.AtualizarTicket(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var ticket = ok.Value as TicketDto;
            Assert.NotNull(ticket);
            Assert.Equal("Ticket Atualizado", ticket.Titulo);
            Assert.Equal(2, ticket.CodigoCliente);
            Assert.Equal("Cliente 2", ticket.NomeCliente);
            Assert.Equal(2, ticket.CodigoModulo);
            Assert.Equal("Modulo 2", ticket.NomeModulo);
            Assert.NotNull(ticket.DataEncerramento);
        }

        [Fact]
        public async Task AtualizarTicket_MantемDataAbertura_Original()
        {
            var dataAberturaOriginal = new DateTime(2026, 1, 1);
            var context = GetDbContext(nameof(AtualizarTicket_MantемDataAbertura_Original));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket Original",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = dataAberturaOriginal
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var dto = new TicketCreateDto
            {
                Titulo = "Ticket Atualizado",
                CodigoCliente = 1,
                CodigoModulo = 1
            };

            var result = await controller.AtualizarTicket(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var ticket = ok.Value as TicketDto;
            Assert.NotNull(ticket);
            Assert.Equal(dataAberturaOriginal, ticket.DataAbertura);
        }

        // ============ TESTES DE DELETE ============

        [Fact]
        public async Task DeletarTicket_RetornaNotFound_QuandoTicketNaoExiste()
        {
            var context = GetDbContext(nameof(DeletarTicket_RetornaNotFound_QuandoTicketNaoExiste));
            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var result = await controller.DeletarTicket(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("999", notFound.Value.ToString());
        }

        [Fact]
        public async Task DeletarTicket_RetornaNoContent_QuandoTicketExiste()
        {
            var context = GetDbContext(nameof(DeletarTicket_RetornaNoContent_QuandoTicketExiste));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket para Deletar",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            var result = await controller.DeletarTicket(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletarTicket_RemoveTicketDoBanco()
        {
            var context = GetDbContext(nameof(DeletarTicket_RemoveTicketDoBanco));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket para Deletar",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            await controller.DeletarTicket(1);

            var ticket = await context.Tickets.FindAsync(1);
            Assert.Null(ticket);
        }

        [Fact]
        public async Task DeletarTicket_NaoAfetaOutrosTickets()
        {
            var context = GetDbContext(nameof(DeletarTicket_NaoAfetaOutrosTickets));
            context.Clientes.Add(new Cliente { Codigo = 1, Nome = "Cliente 1" });
            context.Modulos.Add(new Modulo { Codigo = 1, Nome = "Modulo 1" });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 1,
                Titulo = "Ticket 1",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.Tickets.Add(new TicketDto
            {
                Codigo = 2,
                Titulo = "Ticket 2",
                CodigoCliente = 1,
                CodigoModulo = 1,
                DataAbertura = DateTime.Now
            });
            context.SaveChanges();

            var serviceLogger = new Mock<ILogger<TicketService>>();
            var ticketService = new TicketService(context, serviceLogger.Object);
            var controller = GetController(ticketService);

            await controller.DeletarTicket(1);

            var ticket1 = await context.Tickets.FindAsync(1);
            var ticket2 = await context.Tickets.FindAsync(2);
            Assert.Null(ticket1);
            Assert.NotNull(ticket2);
            Assert.Equal("Ticket 2", ticket2.Titulo);
        }
    }
}