using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SupportInsights.Controllers;
using SupportInsights.Models;
using SupportInsights.Services;
using Xunit;

namespace SupportInsights.Tests
{
    public class DashboardServiceTests
    {
        private DashboardController GetController(Mock<IDashboardService> serviceMock)
        {
            var loggerMock = new Mock<ILogger<DashboardController>>();
            return new DashboardController(loggerMock.Object, serviceMock.Object);
        }

        [Fact]
        public async Task GetDashboard_RetornaBadRequest_QuandoPaginaInvalida()
        {
            var serviceMock = new Mock<IDashboardService>();
            var controller = GetController(serviceMock);

            var result = await controller.GetDashboard(1, 2024, 0, 10);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("página", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetDashboard_RetornaBadRequest_QuandoTamanhoPaginaInvalido()
        {
            var serviceMock = new Mock<IDashboardService>();
            var controller = GetController(serviceMock);

            var result = await controller.GetDashboard(1, 2024, 1, 101);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("tamanho", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetDashboard_RetornaOk_QuandoParametrosValidos()
        {
            var serviceMock = new Mock<IDashboardService>();
            var dashboardResponse = new DashboardResponsePaginado
            {
                Tickets = new List<TicketDto>
                {
                    new TicketDto
                    {
                        Codigo = 1,
                        Titulo = "Teste",
                        CodigoCliente = 1,
                        NomeCliente = "Cliente 1",
                        CodigoModulo = 1,
                        NomeModulo = "Modulo 1",
                        DataAbertura = new DateTime(2024, 1, 10)
                    }
                },
                Paginacao = new PaginationInfo
                {
                    PaginaAtual = 1,
                    TamanhoPagina = 10,
                    TotalItens = 1,
                    TotalPaginas = 1,
                    TemPaginaAnterior = false,
                    TemProximaPagina = false
                }
            };

            serviceMock.Setup(s => s.GetDashboardAsync(1, 2024, 1, 10))
                .ReturnsAsync(dashboardResponse);

            var controller = GetController(serviceMock);

            var result = await controller.GetDashboard(1, 2024, 1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            serviceMock.Verify(s => s.GetDashboardAsync(1, 2024, 1, 10), Times.Once);
        }

        [Fact]
        public async Task GetDashboard_RetornaListaVazia_QuandoNaoHaTickets()
        {
            var serviceMock = new Mock<IDashboardService>();
            var dashboardResponse = new DashboardResponsePaginado
            {
                Tickets = new List<TicketDto>(),
                Paginacao = new PaginationInfo
                {
                    PaginaAtual = 1,
                    TamanhoPagina = 10,
                    TotalItens = 0,
                    TotalPaginas = 0,
                    TemPaginaAnterior = false,
                    TemProximaPagina = false
                }
            };

            serviceMock.Setup(s => s.GetDashboardAsync(1, 2024, 1, 10))
                .ReturnsAsync(dashboardResponse);

            var controller = GetController(serviceMock);

            var result = await controller.GetDashboard(1, 2024, 1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            serviceMock.Verify(s => s.GetDashboardAsync(1, 2024, 1, 10), Times.Once);
        }

        [Fact]
        public async Task GetTicket_RetornaNotFound_QuandoTicketNaoExiste()
        {
            var serviceMock = new Mock<IDashboardService>();
            serviceMock.Setup(s => s.GetTicketByIdAsync(999))
                .ReturnsAsync((TicketDto?)null);

            var controller = GetController(serviceMock);

            var result = await controller.GetTicket(999);

            Assert.IsType<NotFoundResult>(result);
            serviceMock.Verify(s => s.GetTicketByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetTicket_RetornaOk_QuandoTicketExiste()
        {
            var serviceMock = new Mock<IDashboardService>();
            var ticket = new TicketDto
            {
                Codigo = 1,
                Titulo = "Teste",
                CodigoCliente = 1,
                NomeCliente = "Cliente 1",
                CodigoModulo = 1,
                NomeModulo = "Modulo 1",
                DataAbertura = DateTime.Now
            };

            serviceMock.Setup(s => s.GetTicketByIdAsync(1))
                .ReturnsAsync(ticket);

            var controller = GetController(serviceMock);

            var result = await controller.GetTicket(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            serviceMock.Verify(s => s.GetTicketByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetDashboard_FiltraPorMesAno_Corretamente()
        {
            var serviceMock = new Mock<IDashboardService>();
            var dashboardResponse = new DashboardResponsePaginado
            {
                Tickets = new List<TicketDto>
                {
                    new TicketDto
                    {
                        Codigo = 1,
                        Titulo = "Ticket Janeiro",
                        CodigoCliente = 1,
                        NomeCliente = "Cliente 1",
                        CodigoModulo = 1,
                        NomeModulo = "Modulo 1",
                        DataAbertura = new DateTime(2024, 1, 15)
                    }
                },
                Paginacao = new PaginationInfo
                {
                    PaginaAtual = 1,
                    TamanhoPagina = 10,
                    TotalItens = 1,
                    TotalPaginas = 1,
                    TemPaginaAnterior = false,
                    TemProximaPagina = false
                }
            };

            serviceMock.Setup(s => s.GetDashboardAsync(1, 2024, 1, 10))
                .ReturnsAsync(dashboardResponse);

            var controller = GetController(serviceMock);

            var result = await controller.GetDashboard(1, 2024, 1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            serviceMock.Verify(s => s.GetDashboardAsync(1, 2024, 1, 10), Times.Once);
        }
    }
}