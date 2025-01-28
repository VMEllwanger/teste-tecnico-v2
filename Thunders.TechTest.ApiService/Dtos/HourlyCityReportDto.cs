namespace Thunders.TechTest.ApiService.Dtos;

/// <summary>
/// DTO para relatório por hora e cidade
/// </summary>
public class HourlyCityReportDto
{
	/// <summary>
	/// Nome da cidade
	/// </summary>
	public string City { get; set; } = string.Empty;

	/// <summary>
	/// Estado da cidade (UF)
	/// </summary>
	public string State { get; set; } = string.Empty;

	/// <summary>
	/// Data do relatório
	/// </summary>
	public DateTime Date { get; set; }

	/// <summary>
	/// Hora do relatório (0-23)
	/// </summary>
	public int Hour { get; set; }

	/// <summary>
	/// Valor total arrecadado na hora
	/// </summary>
	public decimal TotalAmount { get; set; }

	/// <summary>
	/// Total de veículos na hora
	/// </summary>
	public int TotalVehicles { get; set; }
}
