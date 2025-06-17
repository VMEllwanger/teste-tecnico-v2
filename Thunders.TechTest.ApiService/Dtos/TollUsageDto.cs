using System.Text.Json.Serialization;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Dtos;

/// <summary>
/// DTO para registro de pedágio
/// </summary>
public class TollUsageDto
{
	/// <summary>
	/// Placa do veículo
	/// </summary>
	public string LicensePlate { get; set; } = string.Empty;

	/// <summary>
	/// Tipo do veículo
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public VehicleType VehicleType { get; set; }

	/// <summary>
	/// Data e hora da utilização
	/// </summary>
	public DateTime DateTime { get; set; }

	/// <summary>
	/// Praça de pedágio
	/// </summary>
	public string Plaza { get; set; } = string.Empty;

	/// <summary>
	/// Cidade da praça
	/// </summary>
	public string City { get; set; } = string.Empty;

	/// <summary>
	/// Estado da praça (UF)
	/// </summary>
	public string State { get; set; } = string.Empty;

	/// <summary>
	/// Valor do pedágio
	/// </summary>
	public decimal Amount { get; set; }
}
