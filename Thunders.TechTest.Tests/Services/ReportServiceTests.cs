using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Diagnostics;
using Thunders.TechTest.ApiService.Messaging;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Repository.Interface;
using Thunders.TechTest.ApiService.Services;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.Tests.Services
{
	public class ReportServiceTests
	{
		private readonly IFixture _fixture;
		private readonly IHourlyCityReportRepository _hourlyRepo;
		private readonly IMonthlyPlazaReportRepository _monthlyRepo;
		private readonly IVehicleTypeReportRepository _vehicleTypeRepo;
		private readonly IMessageSender _messageSender;
		private readonly ILogger<ReportService> _logger;
		private readonly ActivitySource _activitySource;
		private readonly ReportService _service;


		public ReportServiceTests()
		{
			_fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
			_hourlyRepo = Substitute.For<IHourlyCityReportRepository>();
			_monthlyRepo = Substitute.For<IMonthlyPlazaReportRepository>();
			_vehicleTypeRepo = Substitute.For<IVehicleTypeReportRepository>();
			_messageSender = Substitute.For<IMessageSender>();
			_logger = Substitute.For<ILogger<ReportService>>();
			_activitySource = new ActivitySource("TestSource");
			_service = new ReportService(_hourlyRepo, _monthlyRepo, _vehicleTypeRepo, _messageSender, _logger, _activitySource);
		}

		[Fact]
		public async Task GetHourlyCityReportAsync_ShouldReturnReports()
		{
			var date = DateTime.UtcNow.Date;
			var reports = _fixture.CreateMany<HourlyCityReport>(3).ToList();
			_hourlyRepo.GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(reports);

			var result = await _service.GetHourlyCityReportAsync(date);

			Assert.Equal(3, result.Count());
			await _hourlyRepo.Received(1).GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
		}

		[Fact]
		public async Task GetHourlyCityReportAsync_WhenException_ShouldLogAndThrow()
		{
			var date = DateTime.UtcNow.Date;
			_hourlyRepo.GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
				.Returns<Task<List<HourlyCityReport>>>(_ => throw new Exception("Erro"));

			await Assert.ThrowsAsync<Exception>(() => _service.GetHourlyCityReportAsync(date));
		}

		[Fact]
		public async Task GetMonthlyPlazaReportAsync_ShouldReturnReports_WithLimit()
		{
			var reports = _fixture.CreateMany<MonthlyPlazaReport>(2).ToList();
			_monthlyRepo.GetByYearAndMonthAsync(2024, 3, 2).Returns(reports);

			var result = await _service.GetMonthlyPlazaReportAsync(2024, 3, 2);

			Assert.Equal(2, result.Count());
			await _monthlyRepo.Received(1).GetByYearAndMonthAsync(2024, 3, 2);
		}

		[Fact]
		public async Task GetMonthlyPlazaReportAsync_WhenException_ShouldLogAndThrow()
		{
			_monthlyRepo.GetByYearAndMonthAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int?>())
				.Returns<Task<List<MonthlyPlazaReport>>>(_ => throw new Exception("Erro"));

			await Assert.ThrowsAsync<Exception>(() => _service.GetMonthlyPlazaReportAsync(2024, 3, 2));
		}

		[Fact]
		public async Task GetVehicleTypeReportAsync_ShouldReturnReports_WithPlaza()
		{
			var date = DateTime.UtcNow.Date;
			var reports = _fixture.CreateMany<VehicleTypeReport>(2).ToList();
			_vehicleTypeRepo.GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), "Plaza1").Returns(reports);

			var result = await _service.GetVehicleTypeReportAsync(date, "Plaza1");

			Assert.Equal(2, result.Count());
			await _vehicleTypeRepo.Received(1).GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), "Plaza1");
		}

		[Fact]
		public async Task GetVehicleTypeReportAsync_WhenException_ShouldLogAndThrow()
		{
			var date = DateTime.UtcNow.Date;
			_vehicleTypeRepo.GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string?>())
				.Returns<Task<List<VehicleTypeReport>>>(_ => throw new Exception("Erro"));

			await Assert.ThrowsAsync<Exception>(() => _service.GetVehicleTypeReportAsync(date, "Plaza1"));
		}

		[Fact]
		public async Task GenerateReportAsync_ShouldSendMessage()
		{
			var date = DateTime.UtcNow.Date;
			var reportType = ReportType.VehicleType;
			_messageSender.SendLocal(Arg.Any<GenerateReportMessage>()).Returns(Task.CompletedTask);

			await _service.GenerateReportAsync(date, reportType);

			await _messageSender.Received(1).SendLocal(Arg.Is<GenerateReportMessage>(m => m.Date == date && m.ReportType == reportType));
		}

		[Fact]
		public async Task GenerateReportAsync_WhenException_ShouldLogAndThrow()
		{
			var date = DateTime.UtcNow.Date;
			var reportType = ReportType.VehicleType;
			_messageSender.SendLocal(Arg.Any<GenerateReportMessage>()).Returns<Task>(_ => throw new Exception("Erro"));

			await Assert.ThrowsAsync<Exception>(() => _service.GenerateReportAsync(date, reportType));
		}

		[Fact]
		public async Task GetHourlyCityReportByIdAsync_ShouldReturnReports()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			var expectedReports = _fixture.CreateMany<HourlyCityReport>(3).ToList();
			_hourlyRepo.GetByReportIdAsync(reportId).Returns(expectedReports.AsEnumerable());

			// Act
			var result = await _service.GetHourlyCityReportByIdAsync(reportId);

			// Assert
			Assert.Equal(3, result.Count());
			await _hourlyRepo.Received(1).GetByReportIdAsync(reportId);
		}

		[Fact]
		public async Task GetHourlyCityReportByIdAsync_WhenException_ShouldThrow()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			_hourlyRepo.GetByReportIdAsync(reportId)
				.Returns<Task<IEnumerable<HourlyCityReport>>>(_ => throw new Exception("Erro ao buscar relatório"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(() => _service.GetHourlyCityReportByIdAsync(reportId));
			await _hourlyRepo.Received(1).GetByReportIdAsync(reportId);
		}

		[Fact]
		public async Task GetMonthlyPlazaReportByIdAsync_WithoutLimit_ShouldReturnAllReports()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			var expectedReports = _fixture.CreateMany<MonthlyPlazaReport>(5).ToList();
			_monthlyRepo.GetByReportIdAsync(reportId).Returns(expectedReports.AsEnumerable());

			// Act
			var result = await _service.GetMonthlyPlazaReportByIdAsync(reportId);

			// Assert
			Assert.Equal(5, result.Count());
			await _monthlyRepo.Received(1).GetByReportIdAsync(reportId);
		}

		[Fact]
		public async Task GetMonthlyPlazaReportByIdAsync_WithLimit_ShouldReturnLimitedReports()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			var limit = 3;
			var expectedReports = _fixture.CreateMany<MonthlyPlazaReport>(5).ToList();
			_monthlyRepo.GetByReportIdAsync(reportId).Returns(expectedReports.AsEnumerable());

			// Act
			var result = await _service.GetMonthlyPlazaReportByIdAsync(reportId, limit);

			// Assert
			Assert.Equal(3, result.Count());
			await _monthlyRepo.Received(1).GetByReportIdAsync(reportId);
		}

		[Fact]
		public async Task GetMonthlyPlazaReportByIdAsync_WithLimitZero_ShouldReturnEmpty()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			var limit = 0;
			var expectedReports = _fixture.CreateMany<MonthlyPlazaReport>(5).ToList();
			_monthlyRepo.GetByReportIdAsync(reportId).Returns(expectedReports.AsEnumerable());

			// Act
			var result = await _service.GetMonthlyPlazaReportByIdAsync(reportId, limit);

			// Assert
			Assert.Empty(result);
			await _monthlyRepo.Received(1).GetByReportIdAsync(reportId);
		}

		[Fact]
		public async Task GetMonthlyPlazaReportByIdAsync_WithLimitGreaterThanTotal_ShouldReturnAllReports()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			var limit = 10;
			var expectedReports = _fixture.CreateMany<MonthlyPlazaReport>(5).ToList();
			_monthlyRepo.GetByReportIdAsync(reportId).Returns(expectedReports.AsEnumerable());

			// Act
			var result = await _service.GetMonthlyPlazaReportByIdAsync(reportId, limit);

			// Assert
			Assert.Equal(5, result.Count());
			await _monthlyRepo.Received(1).GetByReportIdAsync(reportId);
		}

		[Fact]
		public async Task GetMonthlyPlazaReportByIdAsync_WhenException_ShouldThrow()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			_monthlyRepo.GetByReportIdAsync(reportId)
				.Returns<Task<IEnumerable<MonthlyPlazaReport>>>(_ => throw new Exception("Erro ao buscar relatório"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(() => _service.GetMonthlyPlazaReportByIdAsync(reportId));
			await _monthlyRepo.Received(1).GetByReportIdAsync(reportId);
		}

		[Fact]
		public async Task GetVehicleTypeReportByIdAsync_ShouldReturnReports()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			var expectedReports = _fixture.CreateMany<VehicleTypeReport>(4).ToList();
			_vehicleTypeRepo.GetByReportIdAsync(reportId).Returns(expectedReports.AsEnumerable());

			// Act
			var result = await _service.GetVehicleTypeReportByIdAsync(reportId);

			// Assert
			Assert.Equal(4, result.Count());
			await _vehicleTypeRepo.Received(1).GetByReportIdAsync(reportId);
		}

		[Fact]
		public async Task GetVehicleTypeReportByIdAsync_WhenException_ShouldThrow()
		{
			// Arrange
			var reportId = Guid.NewGuid();
			_vehicleTypeRepo.GetByReportIdAsync(reportId)
				.Returns<Task<IEnumerable<VehicleTypeReport>>>(_ => throw new Exception("Erro ao buscar relatório"));

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(() => _service.GetVehicleTypeReportByIdAsync(reportId));
			await _vehicleTypeRepo.Received(1).GetByReportIdAsync(reportId);
		}
	}
}
