using System.ComponentModel.DataAnnotations;

namespace Thunders.TechTest.ApiService.Models;

public class ValidEstadoUFAttribute : ValidationAttribute
{
	private static readonly string[] EstadosValidos = new[]
	{
		"AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG",
		"PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
	};

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value == null)
		{
			return new ValidationResult("O estado é obrigatório");
		}

		var estado = value.ToString()?.ToUpper();
		if (string.IsNullOrEmpty(estado))
		{
			return new ValidationResult("O estado é obrigatório");
		}

		if (!EstadosValidos.Contains(estado))
		{
			return new ValidationResult($"O estado {estado} não é válido");
		}

		return ValidationResult.Success;
	}
}
