using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Dtos;

/// <summary>
/// DTO para relatório por tipo de veículo
/// </summary>
public class VehicleTypeReportDto
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
	/// Estado da praça (UF)
	/// </summary>
	public string State { get; set; } = string.Empty;

	/// <summary>
	/// Data do relatório
	/// </summary>
	public DateTime Date { get; set; }

	/// <summary>
	/// Tipo do veículo
	/// </summary>
	public VehicleType VehicleType { get; set; }

	/// <summary>
	/// Quantidade de veículos do tipo
	/// </summary>
	public int VehicleCount { get; set; }
}
