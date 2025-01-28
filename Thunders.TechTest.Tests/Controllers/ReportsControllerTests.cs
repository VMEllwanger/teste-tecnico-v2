using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Thunders.TechTest.ApiService.Controllers;
using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Services.Interfaces;

namespace Thunders.TechTest.Tests.Controllers;

public class ReportsControllerTests
{
	private readonly IFixture _fixture;
	private readonly IReportService _reportService;
	private readonly ILogger<ReportsController> _logger;
	private readonly ReportsController _controller;

	public ReportsControllerTests()
	{
		_fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
		_reportService = Substitute.For<IReportService>();
		_logger = Substitute.For<ILogger<ReportsController>>();
		_controller = new ReportsController(_reportService, _logger);
	}

	#region GetHourlyCityReportAsync Tests

	[Fact]
	public async Task GetHourlyCityReportAsync_WithValidReportId_ShouldReturnOkWithReports()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		var reports = new List<HourlyCityReport>
		{
			new() { City = "São Paulo", Date = DateTime.Now.AddDays(-1), Hour = 10, TotalAmount = 100, ReportId = reportId },
			new() { City = "Rio de Janeiro", Date = DateTime.Now.AddDays(-1), Hour = 11, TotalAmount = 200, ReportId = reportId }
		};

		_reportService.GetHourlyCityReportByIdAsync(reportId).Returns(reports);

		// Act
		var result = await _controller.GetHourlyCityReportAsync(reportId);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var returnedReports = Assert.IsAssignableFrom<IEnumerable<HourlyCityReportDto>>(okResult.Value);
		Assert.Equal(2, returnedReports.Count());
		await _reportService.Received(1).GetHourlyCityReportByIdAsync(reportId);
	}

	[Fact]
	public async Task GetHourlyCityReportAsync_WithInvalidReportId_ShouldReturnNotFound()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		_reportService.GetHourlyCityReportByIdAsync(reportId).Returns(new List<HourlyCityReport>());

		// Act
		var result = await _controller.GetHourlyCityReportAsync(reportId);

		// Assert
		var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
		Assert.Equal($"Relatório {reportId} não encontrado", notFoundResult.Value);
		await _reportService.Received(1).GetHourlyCityReportByIdAsync(reportId);
	}

	[Fact]
	public async Task GetHourlyCityReportAsync_WhenServiceThrowsException_ShouldThrow()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		_reportService.GetHourlyCityReportByIdAsync(reportId).ThrowsAsync(new Exception("Erro no serviço"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() => _controller.GetHourlyCityReportAsync(reportId));
	}

	#endregion

	#region GetMonthlyPlazaReportAsync Tests

	[Fact]
	public async Task GetMonthlyPlazaReportAsync_WithValidReportId_ShouldReturnOkWithReports()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		var reports = new List<MonthlyPlazaReport>
		{
			new() { Plaza = "Praça 1", Year = 2023, Month = 12, TotalAmount = 1000, Rank = 1, ReportId = reportId },
			new() { Plaza = "Praça 2", Year = 2023, Month = 12, TotalAmount = 800, Rank = 2, ReportId = reportId }
		};

		_reportService.GetMonthlyPlazaReportByIdAsync(reportId, null).Returns(reports);

		// Act
		var result = await _controller.GetMonthlyPlazaReportAsync(reportId);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var returnedReports = Assert.IsAssignableFrom<IEnumerable<MonthlyPlazaReportDto>>(okResult.Value);
		Assert.Equal(2, returnedReports.Count());
		await _reportService.Received(1).GetMonthlyPlazaReportByIdAsync(reportId, null);
	}

	[Fact]
	public async Task GetMonthlyPlazaReportAsync_WithValidReportIdAndLimit_ShouldReturnOkWithLimitedReports()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		var limit = 1;
		var reports = new List<MonthlyPlazaReport>
		{
			new() { Plaza = "Praça 1", Year = 2023, Month = 12, TotalAmount = 1000, Rank = 1, ReportId = reportId },
			new() { Plaza = "Praça 2", Year = 2023, Month = 12, TotalAmount = 800, Rank = 2, ReportId = reportId }
		};

		_reportService.GetMonthlyPlazaReportByIdAsync(reportId, limit).Returns(reports.Take(limit).ToList());

		// Act
		var result = await _controller.GetMonthlyPlazaReportAsync(reportId, limit);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var returnedReports = Assert.IsAssignableFrom<IEnumerable<MonthlyPlazaReportDto>>(okResult.Value);
		Assert.Single(returnedReports);
		await _reportService.Received(1).GetMonthlyPlazaReportByIdAsync(reportId, limit);
	}

	[Fact]
	public async Task GetMonthlyPlazaReportAsync_WithInvalidReportId_ShouldReturnNotFound()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		_reportService.GetMonthlyPlazaReportByIdAsync(reportId, null).Returns(new List<MonthlyPlazaReport>());

		// Act
		var result = await _controller.GetMonthlyPlazaReportAsync(reportId);

		// Assert
		var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
		Assert.Equal($"Relatório {reportId} não encontrado", notFoundResult.Value);
		await _reportService.Received(1).GetMonthlyPlazaReportByIdAsync(reportId, null);
	}

	[Fact]
	public async Task GetMonthlyPlazaReportAsync_WithInvalidLimit_ShouldReturnBadRequest()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		var limit = -1;

		// Act
		var result = await _controller.GetMonthlyPlazaReportAsync(reportId, limit);

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
		Assert.Equal("O limite deve ser maior que zero", badRequestResult.Value);
		await _reportService.DidNotReceive().GetMonthlyPlazaReportByIdAsync(Arg.Any<Guid>(), Arg.Any<int?>());
	}

	[Fact]
	public async Task GetMonthlyPlazaReportAsync_WhenServiceThrowsException_ShouldThrow()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		_reportService.GetMonthlyPlazaReportByIdAsync(reportId, null).ThrowsAsync(new Exception("Erro no serviço"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() => _controller.GetMonthlyPlazaReportAsync(reportId));
	}

	#endregion

	#region GetVehicleTypeReportAsync Tests

	[Fact]
	public async Task GetVehicleTypeReportAsync_WithValidReportId_ShouldReturnOkWithReports()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		var reports = new List<VehicleTypeReport>
		{
			new() { VehicleType = VehicleType.Carro, Date = DateTime.Now.AddDays(-1), VehicleCount = 50, Plaza = "Praça Teste", City = "São Paulo", ReportId = reportId },
			new() { VehicleType = VehicleType.Moto, Date = DateTime.Now.AddDays(-1), VehicleCount = 30, Plaza = "Praça Teste", City = "São Paulo", ReportId = reportId }
		};

		_reportService.GetVehicleTypeReportByIdAsync(reportId).Returns(reports);

		// Act
		var result = await _controller.GetVehicleTypeReportAsync(reportId);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var returnedReports = Assert.IsAssignableFrom<IEnumerable<VehicleTypeReportDto>>(okResult.Value);
		Assert.Equal(2, returnedReports.Count());
		await _reportService.Received(1).GetVehicleTypeReportByIdAsync(reportId);
	}

	[Fact]
	public async Task GetVehicleTypeReportAsync_WithInvalidReportId_ShouldReturnNotFound()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		_reportService.GetVehicleTypeReportByIdAsync(reportId).Returns(new List<VehicleTypeReport>());

		// Act
		var result = await _controller.GetVehicleTypeReportAsync(reportId);

		// Assert
		var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
		Assert.Equal($"Relatório {reportId} não encontrado", notFoundResult.Value);
		await _reportService.Received(1).GetVehicleTypeReportByIdAsync(reportId);
	}

	[Fact]
	public async Task GetVehicleTypeReportAsync_WhenServiceThrowsException_ShouldThrow()
	{
		// Arrange
		var reportId = Guid.NewGuid();
		_reportService.GetVehicleTypeReportByIdAsync(reportId).ThrowsAsync(new Exception("Erro no serviço"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() => _controller.GetVehicleTypeReportAsync(reportId));
	}

	#endregion

	#region GenerateReportAsync Tests

	[Fact]
	public async Task GenerateReportAsync_WithValidParameters_ShouldReturnOkWithReportId()
	{
		// Arrange
		var date = DateTime.Now.AddDays(-1);
		var reportType = ReportType.HourlyCity;
		var reportId = Guid.NewGuid();

		_reportService.GenerateReportAsync(date, reportType).Returns(reportId);

		// Act
		var result = await _controller.GenerateReportAsync(date, reportType);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		Assert.Equal(reportId, okResult.Value);
		await _reportService.Received(1).GenerateReportAsync(date, reportType);
	}

	[Fact]
	public async Task GenerateReportAsync_WithFutureDate_ShouldReturnBadRequest()
	{
		// Arrange
		var futureDate = DateTime.Now.AddDays(1);
		var reportType = ReportType.HourlyCity;

		// Act
		var result = await _controller.GenerateReportAsync(futureDate, reportType);

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
		Assert.Equal("A data não pode ser futura", badRequestResult.Value);
		await _reportService.DidNotReceive().GenerateReportAsync(Arg.Any<DateTime>(), Arg.Any<ReportType>());
	}

	[Fact]
	public async Task GenerateReportAsync_WithInvalidReportType_ShouldReturnBadRequest()
	{
		// Arrange
		var date = DateTime.Now.AddDays(-1);
		var invalidReportType = (ReportType)999;

		// Act
		var result = await _controller.GenerateReportAsync(date, invalidReportType);

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
		Assert.Equal("Tipo de relatório inválido", badRequestResult.Value);
		await _reportService.DidNotReceive().GenerateReportAsync(Arg.Any<DateTime>(), Arg.Any<ReportType>());
	}

	[Fact]
	public async Task GenerateReportAsync_WhenServiceThrowsException_ShouldThrow()
	{
		// Arrange
		var date = DateTime.Now.AddDays(-1);
		var reportType = ReportType.HourlyCity;
		_reportService.GenerateReportAsync(date, reportType).ThrowsAsync(new Exception("Erro no serviço"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() => _controller.GenerateReportAsync(date, reportType));
	}

	[Theory]
	[InlineData(ReportType.HourlyCity)]
	[InlineData(ReportType.MonthlyPlaza)]
	[InlineData(ReportType.VehicleType)]
	public async Task GenerateReportAsync_WithAllValidReportTypes_ShouldReturnOkWithReportId(ReportType reportType)
	{
		// Arrange
		var date = DateTime.Now.AddDays(-1);
		var reportId = Guid.NewGuid();
		_reportService.GenerateReportAsync(date, reportType).Returns(reportId);

		// Act
		var result = await _controller.GenerateReportAsync(date, reportType);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		Assert.Equal(reportId, okResult.Value);
		await _reportService.Received(1).GenerateReportAsync(date, reportType);
	}

	#endregion
}
