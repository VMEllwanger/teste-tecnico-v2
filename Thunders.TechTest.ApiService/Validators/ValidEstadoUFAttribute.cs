using System.ComponentModel.DataAnnotations;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Validators;

/// <summary>
/// Validador customizado para Estados UF brasileiros
/// </summary>
public class ValidEstadoUFAttribute : ValidationAttribute
{
	public ValidEstadoUFAttribute()
	{
		ErrorMessage = "Estado inválido. Informe uma UF válida (ex: SP, RJ, MG)";
	}

	protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
	{
		string[] memberNames = new[] { validationContext?.MemberName ?? "State" };

		if (value == null)
			return new ValidationResult("O estado é obrigatório", memberNames);

		var stringValue = value.ToString();

		if (string.IsNullOrWhiteSpace(stringValue))
			return new ValidationResult("O estado é obrigatório", memberNames);

		if (int.TryParse(stringValue, out _))
			return new ValidationResult(ErrorMessage, memberNames);

		if (!Enum.TryParse<EstadoUF>(stringValue.ToUpper(), out _))
			return new ValidationResult(ErrorMessage, memberNames);

		return ValidationResult.Success;
	}

	public override string FormatErrorMessage(string name)
	{
		var validStates = string.Join(", ", Enum.GetNames<EstadoUF>());
		return $"O campo {name} deve ser um estado válido. Estados válidos: {validStates}";
	}
}
