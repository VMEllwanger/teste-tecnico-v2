using Thunders.TechTest.ApiService.Validators;
using Xunit;

namespace Thunders.TechTest.Tests.Validators;

/// <summary>
/// Testes para o validador de Estados UF
/// </summary>
public class ValidEstadoUFAttributeTests
{
	private readonly ValidEstadoUFAttribute _validator;

	public ValidEstadoUFAttributeTests()
	{
		_validator = new ValidEstadoUFAttribute();
	}

	[Theory]
	[InlineData("SP")]
	[InlineData("RJ")]
	[InlineData("MG")]
	[InlineData("RS")]
	[InlineData("PR")]
	[InlineData("SC")]
	[InlineData("BA")]
	[InlineData("DF")]
	public void IsValid_ValidStates_ShouldReturnTrue(string state)
	{
		// Act
		var result = _validator.IsValid(state);

		// Assert
		Assert.True(result);
	}

	[Theory]
	[InlineData("sp")]
	[InlineData("rj")]
	[InlineData("Sp")]
	public void IsValid_LowercaseStates_ShouldReturnTrue(string state)
	{
		// Act
		var result = _validator.IsValid(state);

		// Assert
		Assert.True(result);
	}

	[Theory]
	[InlineData("XX")]
	[InlineData("YY")]
	[InlineData("ZZ")]
	[InlineData("AB")]
	public void IsValid_InvalidStates_ShouldReturnFalse(string state)
	{
		// Act
		var result = _validator.IsValid(state);

		// Assert
		Assert.False(result);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("   ")]
	public void IsValid_EmptyOrWhitespace_ShouldReturnFalse(string state)
	{
		// Act
		var result = _validator.IsValid(state);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void IsValid_Null_ShouldReturnFalse()
	{
		// Act
		var result = _validator.IsValid(null);

		// Assert
		Assert.False(result);
	}

	[Theory]
	[InlineData("SPP")]
	[InlineData("S")]
	[InlineData("1234")]
	[InlineData("SP1")]
	public void IsValid_InvalidFormat_ShouldReturnFalse(string state)
	{
		// Act
		var result = _validator.IsValid(state);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void FormatErrorMessage_ShouldReturnMessageWithValidStates()
	{
		// Arrange
		var fieldName = "State";

		// Act
		var result = _validator.FormatErrorMessage(fieldName);

		// Assert
		Assert.Contains("State deve ser um estado válido", result);
		Assert.Contains("SP", result);
		Assert.Contains("RJ", result);
		Assert.Contains("MG", result);
	}
}
