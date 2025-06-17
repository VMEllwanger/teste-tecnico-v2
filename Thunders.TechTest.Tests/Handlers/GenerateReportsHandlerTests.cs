using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Diagnostics;
using Thunders.TechTest.ApiService.Handlers;
using Thunders.TechTest.ApiService.Messaging;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Repository.Interface;

namespace Thunders.TechTest.Tests.Handlers;

public class GenerateReportsHandlerTests
{
	private readonly IFixture _fixture;
	private readonly ITollUsageRepository _tollUsageRepository;
	private readonly IHourlyCityReportRepository _hourlyCityReportRepository;
	private readonly IMonthlyPlazaReportRepository _monthlyPlazaReportRepository;
	private readonly IVehicleTypeReportRepository _vehicleTypeReportRepository;
	private readonly ILogger<GenerateReportsHandler> _logger;
	private readonly ActivitySource _activitySource;
	private readonly GenerateReportsHandler _handler;

	public GenerateReportsHandlerTests()
	{
		_fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
		_tollUsageRepository = Substitute.For<ITollUsageRepository>();
		_hourlyCityReportRepository = Substitute.For<IHourlyCityReportRepository>();
		_monthlyPlazaReportRepository = Substitute.For<IMonthlyPlazaReportRepository>();
		_vehicleTypeReportRepository = Substitute.For<IVehicleTypeReportRepository>();
		_logger = Substitute.For<ILogger<GenerateReportsHandler>>();
		_activitySource = new ActivitySource("Test");
		_handler = new GenerateReportsHandler(
			_tollUsageRepository,
			_hourlyCityReportRepository,
			_monthlyPlazaReportRepository,
			_vehicleTypeReportRepository,
			_logger,
			_activitySource);
	}

	[Fact]
	public async Task Handle_WhenHourlyCityReport_ShouldGenerateReport()
	{
		// Arrange
		var message = new GenerateReportMessage(Guid.NewGuid(), DateTime.Now, ReportType.HourlyCity);
		var tollUsages = new List<TollUsage>
		{
			new() { City = "São Paulo", Plaza = "Praça 1", LicensePlate = "ABC1234", CreatedAt = DateTime.UtcNow, Amount = 10 },
			new() { City = "São Paulo", Plaza = "Praça 1", LicensePlate = "DEF5678", CreatedAt = DateTime.UtcNow, Amount = 20 },
			new() { City = "Rio de Janeiro", Plaza = "Praça 2", LicensePlate = "GHI9012", CreatedAt = DateTime.UtcNow, Amount = 15 }
		};

		_tollUsageRepository.GetByDateAsync(Arg.Any<DateTime>()).Returns(tollUsages);
		_hourlyCityReportRepository.GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(Task.FromResult(new List<HourlyCityReport>()));

		// Act
		await _handler.Handle(message);

		// Assert
		await _hourlyCityReportRepository.Received(1).AddAsync(Arg.Is<HourlyCityReport>(r =>
			r.City == "São Paulo" &&
			r.TotalAmount == 30 &&
			r.TotalVehicles == 2));
		await _hourlyCityReportRepository.Received(1).AddAsync(Arg.Is<HourlyCityReport>(r =>
			r.City == "Rio de Janeiro" &&
			r.TotalAmount == 15 &&
			r.TotalVehicles == 1));
		await _hourlyCityReportRepository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task Handle_WhenMonthlyPlazaReport_ShouldGenerateReport()
	{
		// Arrange
		var message = new GenerateReportMessage(Guid.NewGuid(), DateTime.Now, ReportType.MonthlyPlaza);
		var tollUsages = new List<TollUsage>
		{
			new() { Plaza = "Praça 1", City = "São Paulo", State = "SP", LicensePlate = "ABC1234", CreatedAt = DateTime.UtcNow, Amount = 10 },
			new() { Plaza = "Praça 1", City = "São Paulo", State = "SP", LicensePlate = "DEF5678", CreatedAt = DateTime.UtcNow, Amount = 20 },
			new() { Plaza = "Praça 2", City = "Rio de Janeiro", State = "RJ", LicensePlate = "GHI9012", CreatedAt = DateTime.UtcNow, Amount = 15 }
		};

		_tollUsageRepository.GetByMonthAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(tollUsages);
		_monthlyPlazaReportRepository.GetByYearAndMonthAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int?>()).Returns(Task.FromResult(new List<MonthlyPlazaReport>()));

		// Act
		await _handler.Handle(message);

		// Assert
		await _monthlyPlazaReportRepository.Received(1).AddAsync(Arg.Is<MonthlyPlazaReport>(r =>
			r.Plaza == "Praça 1" &&
			r.TotalAmount == 30 &&
			r.Rank == 1));
		await _monthlyPlazaReportRepository.Received(1).AddAsync(Arg.Is<MonthlyPlazaReport>(r =>
			r.Plaza == "Praça 2" &&
			r.TotalAmount == 15 &&
			r.Rank == 2));
		await _monthlyPlazaReportRepository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task Handle_WhenVehicleTypeReport_ShouldGenerateReport()
	{
		// Arrange
		var messageDate = DateTime.Today;
		var message = new GenerateReportMessage(Guid.NewGuid(), messageDate, ReportType.VehicleType);
		var utcDate = DateTime.SpecifyKind(messageDate.Date, DateTimeKind.Local).ToUniversalTime();

		var tollUsages = new List<TollUsage>
		{
			new() { VehicleType = VehicleType.Carro, Plaza = "Praça 1", City = "São Paulo", State = "SP", LicensePlate = "ABC1234", CreatedAt = utcDate, Amount = 10, DateTime = utcDate },
			new() { VehicleType = VehicleType.Carro, Plaza = "Praça 1", City = "São Paulo", State = "SP", LicensePlate = "DEF5678", CreatedAt = utcDate, Amount = 20, DateTime = utcDate },
			new() { VehicleType = VehicleType.Moto, Plaza = "Praça 2", City = "Rio de Janeiro", State = "RJ", LicensePlate = "GHI9012", CreatedAt = utcDate, Amount = 15, DateTime = utcDate }
		};

		_tollUsageRepository.GetByDateAsync(messageDate).Returns(tollUsages);
		_vehicleTypeReportRepository.GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(Task.FromResult(new List<VehicleTypeReport>()));

		// Act
		await _handler.Handle(message);

		// Assert
		_vehicleTypeReportRepository.Received(1).AddAsync(Arg.Is<VehicleTypeReport>(r =>
			r.VehicleType == VehicleType.Carro &&
			r.VehicleCount == 2));
		_vehicleTypeReportRepository.Received(1).AddAsync(Arg.Is<VehicleTypeReport>(r =>
			r.VehicleType == VehicleType.Moto &&
			r.VehicleCount == 1));
		_vehicleTypeReportRepository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task Handle_WhenInvalidReportType_ShouldThrowArgumentException()
	{
		// Arrange
		var message = new GenerateReportMessage(Guid.NewGuid(), DateTime.Now, (ReportType)999);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(message));
	}

	[Fact]
	public async Task Handle_WhenExceptionOccurs_ShouldThrow()
	{
		// Arrange
		var message = new GenerateReportMessage(Guid.NewGuid(), DateTime.Now, ReportType.HourlyCity);
		_tollUsageRepository.GetByDateAsync(Arg.Any<DateTime>())
			.ThrowsAsync(new Exception("Erro"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() => _handler.Handle(message));
	}

	[Fact]
	public async Task Handle_WhenExistingHourlyCityReports_ShouldRemoveAndCreateNew()
	{
		// Arrange
		var message = new GenerateReportMessage(Guid.NewGuid(), DateTime.Now, ReportType.HourlyCity);
		var oldDate = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Local).ToUniversalTime();
		var existingReports = new List<HourlyCityReport>
		{
			new() { City = "São Paulo", Date = oldDate, Hour = 10, TotalAmount = 100 },
			new() { City = "Rio de Janeiro", Date = oldDate, Hour = 11, TotalAmount = 200 }
		};

		var tollUsages = new List<TollUsage>
		{
			new() { City = "São Paulo", Plaza = "Praça 1", LicensePlate = "ABC1234", CreatedAt = oldDate, Amount = 10 },
			new() { City = "Rio de Janeiro", Plaza = "Praça 2", LicensePlate = "DEF5678", CreatedAt = oldDate, Amount = 20 }
		};

		_tollUsageRepository.GetByDateAsync(Arg.Any<DateTime>()).Returns(tollUsages);
		_hourlyCityReportRepository.GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(Task.FromResult(existingReports));

		// Act
		await _handler.Handle(message);

		// Assert
		await _hourlyCityReportRepository.Received(1).AddAsync(Arg.Is<HourlyCityReport>(r =>
			r.City == "São Paulo" &&
			r.TotalAmount == 10));
		await _hourlyCityReportRepository.Received(1).AddAsync(Arg.Is<HourlyCityReport>(r =>
			r.City == "Rio de Janeiro" &&
			r.TotalAmount == 20));
		await _hourlyCityReportRepository.Received().SaveChangesAsync();
	}

	[Fact]
	public async Task Handle_WhenExistingMonthlyPlazaReports_ShouldRemoveAndCreateNew()
	{
		// Arrange
		var message = new GenerateReportMessage(Guid.NewGuid(), DateTime.Now, ReportType.MonthlyPlaza);
		var existingReports = new List<MonthlyPlazaReport>
		{
			new() { Plaza = "Praça 1", Year = DateTime.UtcNow.Year, Month = DateTime.UtcNow.Month, TotalAmount = 100 },
			new() { Plaza = "Praça 2", Year = DateTime.UtcNow.Year, Month = DateTime.UtcNow.Month, TotalAmount = 200 }
		};

		var tollUsages = new List<TollUsage>
		{
			new() { Plaza = "Praça 1", City = "São Paulo", State = "SP", LicensePlate = "ABC1234", CreatedAt = DateTime.UtcNow, Amount = 10 },
			new() { Plaza = "Praça 2", City = "Rio de Janeiro", State = "RJ", LicensePlate = "DEF5678", CreatedAt = DateTime.UtcNow, Amount = 20 }
		};

		_tollUsageRepository.GetByMonthAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(tollUsages);
		_monthlyPlazaReportRepository.GetByYearAndMonthAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int?>()).Returns(Task.FromResult(existingReports));

		// Act
		await _handler.Handle(message);

		// Assert
		await _monthlyPlazaReportRepository.Received(1).AddAsync(Arg.Is<MonthlyPlazaReport>(r =>
			r.Plaza == "Praça 1" &&
			r.TotalAmount == 10));
		await _monthlyPlazaReportRepository.Received(1).AddAsync(Arg.Is<MonthlyPlazaReport>(r =>
			r.Plaza == "Praça 2" &&
			r.TotalAmount == 20));
		await _monthlyPlazaReportRepository.Received().SaveChangesAsync();
	}

	[Fact]
	public async Task Handle_WhenExistingVehicleTypeReports_ShouldRemoveAndCreateNew()
	{
		// Arrange
		var messageDate = DateTime.Today;
		var message = new GenerateReportMessage(Guid.NewGuid(), messageDate, ReportType.VehicleType);
		var utcDate = DateTime.SpecifyKind(messageDate.Date, DateTimeKind.Local).ToUniversalTime();

		var existingReports = new List<VehicleTypeReport>
		{
			new() { VehicleType = VehicleType.Carro, Date = utcDate, VehicleCount = 100, Plaza = "Praça 1", City = "São Paulo", State = "SP" },
			new() { VehicleType = VehicleType.Moto, Date = utcDate, VehicleCount = 200, Plaza = "Praça 2", City = "Rio de Janeiro", State = "RJ" }
		};

		var tollUsages = new List<TollUsage>
		{
			new() { VehicleType = VehicleType.Carro, Plaza = "Praça 1", City = "São Paulo", State = "SP", LicensePlate = "ABC1234", CreatedAt = utcDate, Amount = 10, DateTime = utcDate },
			new() { VehicleType = VehicleType.Moto, Plaza = "Praça 2", City = "Rio de Janeiro", State = "RJ", LicensePlate = "DEF5678", CreatedAt = utcDate, Amount = 20, DateTime = utcDate }
		};

		_tollUsageRepository.GetByDateAsync(messageDate).Returns(tollUsages);
		_vehicleTypeReportRepository.GetByDateRangeAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(Task.FromResult(existingReports));

		// Act
		await _handler.Handle(message);

		// Assert
		_vehicleTypeReportRepository.Received(2).Remove(Arg.Any<VehicleTypeReport>());

		_vehicleTypeReportRepository.Received().AddAsync(Arg.Any<VehicleTypeReport>());

		_vehicleTypeReportRepository.Received(2).SaveChangesAsync();
	}
}
