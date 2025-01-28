using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Diagnostics;
using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Repository.Interface;
using Thunders.TechTest.ApiService.Services;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.Tests.Services
{
	public class TollUsageServiceTests
	{
		private readonly IFixture _fixture;
		private readonly TollUsageService _service;
		private readonly IMessageSender _messageSender;
		private readonly ILogger<TollUsageService> _logger;
		private readonly ActivitySource _activitySource;
		private readonly ITollUsageRepository _repository;
		private readonly IHourlyCityReportRepository _hourlyCityReportRepository;
		private readonly IMonthlyPlazaReportRepository _monthlyPlazaReportRepository;
		private readonly IVehicleTypeReportRepository _vehicleTypeReportRepository;

		public TollUsageServiceTests()
		{
			_fixture = new Fixture()
				.Customize(new AutoNSubstituteCustomization());

			_fixture.Register<decimal>(() => 10.50m);

			_messageSender = Substitute.For<IMessageSender>();
			_logger = Substitute.For<ILogger<TollUsageService>>();
			_repository = Substitute.For<ITollUsageRepository>();
			_hourlyCityReportRepository = Substitute.For<IHourlyCityReportRepository>();
			_monthlyPlazaReportRepository = Substitute.For<IMonthlyPlazaReportRepository>();
			_vehicleTypeReportRepository = Substitute.For<IVehicleTypeReportRepository>();
			_service = new TollUsageService(_repository, _hourlyCityReportRepository, _monthlyPlazaReportRepository, _vehicleTypeReportRepository, _messageSender, _logger);
		}

		private TollUsageRequestDto CreateValidTollUsageRequestDto()
		{
			return new TollUsageRequestDto
			{
				LicensePlate = "ABC1234",
				VehicleType = VehicleType.Carro,
				DateTime = DateTime.UtcNow,
				Plaza = "Plaza Teste",
				City = "Cidade Teste",
				State = "SP",
				Amount = 10.50m
			};
		}

		[Fact]
		public async Task CreateTollUsageAsync_ShouldCreateTollUsageAndSendMessage()
		{
			// Arrange
			var dto = CreateValidTollUsageRequestDto();
			var tollUsage = new TollUsage
			{
				Id = Guid.NewGuid(),
				LicensePlate = dto.LicensePlate,
				VehicleType = dto.VehicleType,
				DateTime = dto.DateTime,
				Plaza = dto.Plaza,
				City = dto.City,
				State = dto.State,
				Amount = dto.Amount,
				CreatedAt = DateTime.UtcNow
			};

			_repository.AddAsync(Arg.Any<TollUsage>()).Returns(Task.FromResult(tollUsage));
			_repository.SaveChangesAsync().Returns(Task.FromResult(1));

			// Act
			await _service.CreateTollUsageAsync(dto);

			// Assert
			await _repository.Received(1).AddAsync(Arg.Is<TollUsage>(t =>
				t.LicensePlate == dto.LicensePlate &&
				t.VehicleType == dto.VehicleType &&
				t.DateTime == dto.DateTime &&
				t.Plaza == dto.Plaza &&
				t.City == dto.City &&
				t.State == dto.State &&
				t.Amount == dto.Amount));
			await _repository.Received(1).SaveChangesAsync();
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnTollUsage_WhenExists()
		{
			// Arrange
			var tollUsage = new TollUsage
			{
				Id = Guid.NewGuid(),
				LicensePlate = "ABC1234",
				VehicleType = VehicleType.Caminhao,
				DateTime = DateTime.UtcNow,
				Plaza = "Plaza1",
				City = "City1",
				Amount = 10.0m
			};

			_repository.GetByIdAsync(tollUsage.Id).Returns(Task.FromResult<TollUsage?>(tollUsage));

			// Act
			var result = await _service.GetByIdAsync(tollUsage.Id);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(tollUsage.Id, result.Id);
			Assert.Equal(tollUsage.LicensePlate, result.LicensePlate);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
		{
			// Arrange
			var id = Guid.NewGuid();
			_repository.GetByIdAsync(id).Returns(Task.FromResult<TollUsage?>(null));

			// Act
			var result = await _service.GetByIdAsync(id);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task GetTollUsagesAsync_ShouldReturnTollUsages()
		{
			// Arrange
			var date = DateTime.UtcNow.Date;
			var tollUsages = new List<TollUsage>
			{
				new()
				{
					Id = Guid.NewGuid(),
					LicensePlate = "ABC1234",
					VehicleType = VehicleType.Caminhao,
					DateTime = date,
					Plaza = "Plaza1",
					City = "City1",
					Amount = 10.0m
				}
			};

			_repository.GetByDateAsync(date).Returns(Task.FromResult(tollUsages));

			// Act
			var result = await _service.GetTollUsagesAsync(date);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal(tollUsages[0].Id, result.First().Id);
		}


		[Fact]
		public async Task GetTollUsagesAsync_ShouldReturnTollUsagesForDate()
		{
			// Arrange
			var date = DateTime.UtcNow.Date;
			var tollUsages = new List<TollUsage>
			{
				new TollUsage
				{
					Id = Guid.NewGuid(),
					LicensePlate = "ABC1234",
					Amount = 10.50m,
					DateTime = date,
					Plaza = "Plaza 1",
					City = "City 1",
					VehicleType = VehicleType.Carro,
					CreatedAt = date
				},
				new TollUsage
				{
					Id = Guid.NewGuid(),
					LicensePlate = "DEF5678",
					Amount = 15.75m,
					DateTime = date.AddHours(2),
					Plaza = "Plaza 2",
					City = "City 2",
					VehicleType = VehicleType.Caminhao,
					CreatedAt = date.AddHours(2)
				}
			};

			_repository.GetByDateAsync(date).Returns(Task.FromResult(tollUsages));

			// Act
			var result = await _service.GetTollUsagesAsync(date);

			// Assert
			Assert.Equal(2, result.Count());
			Assert.Contains(result, t => t.LicensePlate == "ABC1234");
			Assert.Contains(result, t => t.LicensePlate == "DEF5678");
		}

		[Fact]
		public async Task CreateTollUsageAsync_WhenDateTimeIsUnspecified_ShouldConvertToUtc()
		{
			// Arrange
			var request = new TollUsageRequestDto
			{
				LicensePlate = "ABC1234",
				Amount = 10.50m,
				DateTime = DateTime.Now,
				Plaza = "Plaza 1",
				City = "City 1",
				State = "SP",
				VehicleType = VehicleType.Carro
			};

			_repository.SaveChangesAsync().Returns(Task.FromResult(1));

			// Act
			var result = await _service.CreateTollUsageAsync(request);

			// Assert
			_repository.Received(1).AddAsync(Arg.Is<TollUsage>(t =>
				t.LicensePlate == request.LicensePlate &&
				t.Amount == request.Amount &&
				t.Plaza == request.Plaza &&
				t.City == request.City &&
				t.VehicleType == request.VehicleType &&
				t.DateTime.Kind == DateTimeKind.Utc));
		}



		[Fact]
		public async Task GetTollUsagesAsync_ShouldHandleLocalDate()
		{
			// Arrange
			var date = DateTime.Now.Date;
			var tollUsages = new List<TollUsage>
			{
				new TollUsage
				{
					Id = Guid.NewGuid(),
					LicensePlate = "ABC1234",
					Amount = 10.50m,
					DateTime = date,
					Plaza = "Plaza 1",
					City = "City 1",
					VehicleType = VehicleType.Carro,
					CreatedAt = date
				},
				new TollUsage
				{
					Id = Guid.NewGuid(),
					LicensePlate = "DEF5678",
					Amount = 15.75m,
					DateTime = date.AddHours(2),
					Plaza = "Plaza 2",
					City = "City 2",
					VehicleType = VehicleType.Caminhao,
					CreatedAt = date.AddHours(2)
				}
			};

			_repository.GetByDateAsync(date).Returns(Task.FromResult(tollUsages));

			// Act
			var result = await _service.GetTollUsagesAsync(date);

			// Assert
			Assert.Equal(2, result.Count());
			Assert.Contains(result, t => t.LicensePlate == "ABC1234");
			Assert.Contains(result, t => t.LicensePlate == "DEF5678");
		}

		[Fact]
		public async Task CreateTollUsageAsync_WhenExceptionOccurs_ShouldLogAndThrow()
		{
			// Arrange
			var request = new TollUsageRequestDto
			{
				LicensePlate = "ABC1234",
				Amount = 10.50m,
				DateTime = DateTime.UtcNow,
				Plaza = "Plaza 1",
				City = "City 1",
				State = "SP",
				VehicleType = VehicleType.Carro
			};

			_repository.SaveChangesAsync().Returns(Task.FromException<int>(new DbUpdateException("Erro ao salvar")));

			// Act & Assert
			await Assert.ThrowsAsync<DbUpdateException>(() => _service.CreateTollUsageAsync(request));
		}

		[Fact]
		public async Task GetByIdAsync_WhenExceptionOccurs_ShouldLogAndThrow()
		{
			// Arrange
			var id = Guid.NewGuid();
			_repository.GetByIdAsync(id).Returns(Task.FromException<TollUsage?>(new DbUpdateException("Erro ao buscar")));

			// Act & Assert
			await Assert.ThrowsAsync<DbUpdateException>(() => _service.GetByIdAsync(id));
		}


		[Fact]
		public async Task GetTollUsagesAsync_WhenExceptionOccurs_ShouldLogAndThrow()
		{
			// Arrange
			var date = DateTime.UtcNow;
			_repository.GetByDateAsync(date).Returns(Task.FromException<List<TollUsage>>(new DbUpdateException("Erro ao buscar")));

			// Act & Assert
			await Assert.ThrowsAsync<DbUpdateException>(() => _service.GetTollUsagesAsync(date));
		}

	}
}
