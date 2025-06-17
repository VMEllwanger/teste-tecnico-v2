namespace Thunders.TechTest.ApiService.Dtos;

/// <summary>
/// DTO para relatório mensal por praça
/// </summary>
public class MonthlyPlazaReportDto
{
	/// <summary>
	/// Nome da praça
	/// </summary>
	public string Plaza { get; set; } = string.Empty;

	/// <summary>
	/// Nome da cidade
	/// </summary>
	public string City { get; set; } = string.Empty;

	/// <summary>
	/// Estado da praça
	/// </summary>
	public string State { get; set; } = string.Empty;

	/// <summary>
	/// Ano do relatório
	/// </summary>
	public int Year { get; set; }

	/// <summary>
	/// Mês do relatório
	/// </summary>
	public int Month { get; set; }

	/// <summary>
	/// Valor total arrecadado no mês
	/// </summary>
	public decimal TotalAmount { get; set; }

	/// <summary>
	/// Ranking da praça no mês
	/// </summary>
	public int Rank { get; set; }
}
