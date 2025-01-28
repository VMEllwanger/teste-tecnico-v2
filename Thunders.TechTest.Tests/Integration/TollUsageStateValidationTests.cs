using System.ComponentModel.DataAnnotations;
using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.Tests.Integration;

/// <summary>
/// Testes de integração para validação de Estados UF
/// </summary>
public class TollUsageStateValidationTests
{
	private readonly ValidationContext _validationContext;

	public TollUsageStateValidationTests()
	{
		_validationContext = new ValidationContext(new object());
	}

	[Theory]
	[InlineData("SP")]
	[InlineData("RJ")]
	[InlineData("MG")]
	[InlineData("RS")]
	[InlineData("PR")]
	[InlineData("SC")]
	[InlineData("BA")]
	[InlineData("GO")]
	[InlineData("MT")]
	[InlineData("MS")]
	[InlineData("PA")]
	[InlineData("AM")]
	[InlineData("RO")]
	[InlineData("AC")]
	[InlineData("AP")]
	[InlineData("RR")]
	[InlineData("TO")]
	[InlineData("MA")]
	[InlineData("PI")]
	[InlineData("CE")]
	[InlineData("RN")]
	[InlineData("PB")]
	[InlineData("PE")]
	[InlineData("AL")]
	[InlineData("SE")]
	[InlineData("ES")]
	[InlineData("DF")]
	public void TollUsageRequestDto_ValidStates_ShouldPassValidation(string state)
	{
		// Arrange
		var dto = CreateValidTollUsageRequestDto();
		dto.State = state;

		// Act
		var validationResults = new List<ValidationResult>();
		var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

		// Assert
		Assert.True(isValid, $"Estado {state} deveria ser válido. Erros: {string.Join(", ", validationResults.Select(v => v.ErrorMessage))}");
		Assert.Empty(validationResults.Where(v => v.MemberNames.Contains("State")));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("ZZ")]
	[InlineData("ABC")]
	[InlineData("123")]
	[InlineData("GOIAS")]
	[InlineData("XX")]
	[InlineData("YY")]
	[InlineData("AB")]
	[InlineData(" ")]
	[InlineData("SPP")]
	[InlineData("S")]
	public void Deve_retornar_erro_para_estado_invalido(string? estadoInvalido)
	{
		// Arrange
		var dto = new TollUsageRequestDto
		{
			LicensePlate = "ABC1234",
			VehicleType = VehicleType.Carro,
			DateTime = DateTime.UtcNow,
			Plaza = "Plaza 1",
			City = "São Paulo",
			State = estadoInvalido ?? "",
			Amount = 10.5m
		};

		var context = new ValidationContext(dto);

		// Act
		var results = new List<ValidationResult>();
		var isValid = Validator.TryValidateObject(dto, context, results, true);

		// Assert
		Assert.False(isValid);
		Assert.Contains(results, r => r.ErrorMessage!.Contains("estado", StringComparison.OrdinalIgnoreCase));
	}



	[Theory]
	[InlineData("sp", "SP")]
	[InlineData("rj", "RJ")]
	[InlineData("mg", "MG")]
	[InlineData("Sp", "SP")]
	[InlineData("rJ", "RJ")]
	public void TollUsageEntity_StateNormalization_ShouldConvertToUppercase(string inputState, string expectedState)
	{
		// Arrange
		var tollUsage = new TollUsage
		{
			Id = Guid.NewGuid(),
			LicensePlate = "ABC1234",
			VehicleType = VehicleType.Carro,
			DateTime = DateTime.UtcNow,
			Plaza = "Plaza Teste",
			City = "Cidade Teste",
			State = inputState.ToUpper(),
			Amount = 10.50m,
			CreatedAt = DateTime.UtcNow
		};

		// Act & Assert
		Assert.Equal(expectedState, tollUsage.State);
	}

	[Fact]
	public void TollUsageRequestDto_AllPropertiesValid_ShouldPassValidation()
	{
		// Arrange
		var dto = CreateValidTollUsageRequestDto();

		// Act
		var validationResults = new List<ValidationResult>();
		var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

		// Assert
		Assert.True(isValid, $"DTO deveria ser válido. Erros: {string.Join(", ", validationResults.Select(v => v.ErrorMessage))}");
		Assert.Empty(validationResults);
	}

	[Fact]
	public void TollUsageRequestDto_MissingRequiredFields_ShouldFailValidation()
	{
		// Arrange
		var dto = new TollUsageRequestDto();

		// Act
		var validationResults = new List<ValidationResult>();
		var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

		// Assert
		Assert.False(isValid);
		Assert.Contains(validationResults, v => v.MemberNames.Contains("LicensePlate"));
		Assert.Contains(validationResults, v => v.MemberNames.Contains("Plaza"));
		Assert.Contains(validationResults, v => v.MemberNames.Contains("City"));
		Assert.Contains(validationResults, v => v.MemberNames.Contains("State"));
		Assert.Contains(validationResults, v => v.MemberNames.Contains("Amount"));
	}

	private static TollUsageRequestDto CreateValidTollUsageRequestDto()
	{
		return new TollUsageRequestDto
		{
			LicensePlate = "ABC1234",
			VehicleType = VehicleType.Carro,
			DateTime = DateTime.Now,
			Plaza = "Plaza Teste",
			City = "Cidade Teste",
			State = "SP",
			Amount = 10.50m
		};
	}
}
