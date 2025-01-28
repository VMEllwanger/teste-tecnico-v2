using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Thunders.TechTest.ApiService.Controllers;
using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Services.Interfaces;

namespace Thunders.TechTest.Tests.Controllers;

public class TollUsageControllerTests
{
  private readonly IFixture _fixture;
  private readonly ITollUsageService _tollUsageService;
  private readonly ILogger<TollUsageController> _logger;
  private readonly TollUsageController _controller;

  public TollUsageControllerTests()
  {
    _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
    _tollUsageService = Substitute.For<ITollUsageService>();
    _logger = Substitute.For<ILogger<TollUsageController>>();
    _controller = new TollUsageController(_tollUsageService, _logger);
  }

  [Fact]
  public async Task CreateAsync_WithValidData_ShouldReturnOkWithId()
  {
    // Arrange
    var tollUsageDto = new TollUsageRequestDto
    {
      LicensePlate = "ABC1234",
      Plaza = "Praça Teste",
      City = "São Paulo",
      State = "SP",
      VehicleType = VehicleType.Carro,
      Amount = 10.50M
    };
    var expectedId = Guid.NewGuid();

    _tollUsageService.CreateTollUsageAsync(tollUsageDto).Returns(expectedId);

    // Act
    var result = await _controller.CreateAsync(tollUsageDto);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    Assert.Equal(expectedId, okResult.Value);
    await _tollUsageService.Received(1).CreateTollUsageAsync(tollUsageDto);
  }

  [Fact]
  public async Task CreateAsync_WithInvalidModelState_ShouldReturnBadRequest()
  {
    // Arrange
    var tollUsageDto = new TollUsageRequestDto();
    _controller.ModelState.AddModelError("LicensePlate", "Campo obrigatório");

    // Act
    var result = await _controller.CreateAsync(tollUsageDto);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result.Result);
    await _tollUsageService.DidNotReceive().CreateTollUsageAsync(Arg.Any<TollUsageRequestDto>());
  }

  [Fact]
  public async Task CreateAsync_WhenServiceThrowsException_ShouldThrow()
  {
    // Arrange
    var tollUsageDto = new TollUsageRequestDto
    {
      LicensePlate = "ABC1234",
      Plaza = "Praça Teste",
      City = "São Paulo",
      State = "SP",
      VehicleType = VehicleType.Carro,
      Amount = 10.50M
    };

    _tollUsageService.CreateTollUsageAsync(tollUsageDto).ThrowsAsync(new Exception("Erro no serviço"));

    // Act & Assert
    await Assert.ThrowsAsync<Exception>(() => _controller.CreateAsync(tollUsageDto));
  }

  [Fact]
  public async Task GetByDateAsync_WithValidDate_ShouldReturnOkWithTollUsages()
  {
    // Arrange
    var date = DateTime.Now.AddDays(-1);
    var tollUsages = new List<TollUsage>
        {
            new() { Id = Guid.NewGuid(), LicensePlate = "ABC1234", Plaza = "Praça 1", City = "São Paulo", State = "SP", Amount = 10.50m, VehicleType = VehicleType.Carro, DateTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), LicensePlate = "DEF5678", Plaza = "Praça 2", City = "Rio de Janeiro", State = "RJ", Amount = 15.75m, VehicleType = VehicleType.Caminhao, DateTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        };

    _tollUsageService.GetTollUsagesAsync(Arg.Any<DateTime>()).Returns(tollUsages);

    // Act
    var result = await _controller.GetByDateAsync(date);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var returnedTollUsages = Assert.IsAssignableFrom<IEnumerable<TollUsageDto>>(okResult.Value);
    Assert.Equal(2, returnedTollUsages.Count());
    await _tollUsageService.Received(1).GetTollUsagesAsync(Arg.Any<DateTime>());
  }

  [Fact]
  public async Task GetByDateAsync_WithFutureDate_ShouldReturnBadRequest()
  {
    // Arrange
    var futureDate = DateTime.Now.AddDays(1);

    // Act
    var result = await _controller.GetByDateAsync(futureDate);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    Assert.Equal("A data não pode ser futura", badRequestResult.Value);
    await _tollUsageService.DidNotReceive().GetTollUsagesAsync(Arg.Any<DateTime>());
  }

  [Fact]
  public async Task GetByDateAsync_WhenServiceThrowsException_ShouldThrow()
  {
    // Arrange
    var date = DateTime.Now.AddDays(-1);
    _tollUsageService.GetTollUsagesAsync(Arg.Any<DateTime>()).ThrowsAsync(new Exception("Erro no serviço"));

    // Act & Assert
    await Assert.ThrowsAsync<Exception>(() => _controller.GetByDateAsync(date));
  }

  [Fact]
  public async Task GetByDateAsync_WithValidDate_ShouldCallServiceWithLocalDate()
  {
    // Arrange
    var date = new DateTime(2023, 12, 15);
    var tollUsages = new List<TollUsage>();

    _tollUsageService.GetTollUsagesAsync(Arg.Any<DateTime>()).Returns(tollUsages);

    // Act
    await _controller.GetByDateAsync(date);

    // Assert
    await _tollUsageService.Received(1).GetTollUsagesAsync(
        Arg.Is<DateTime>(d => d.Date == date.Date && d.Kind == DateTimeKind.Local));
  }
}
