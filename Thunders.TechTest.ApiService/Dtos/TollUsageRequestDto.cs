using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Dtos
{
	/// <summary>
	/// DTO para o request de utilização de pedágio
	/// </summary>
	public class TollUsageRequestDto
	{
		/// <summary>
		/// Placa do veículo
		/// </summary>
		[Required(ErrorMessage = "A placa do veículo é obrigatória")]
		[RegularExpression(@"^[A-Z]{3}[0-9]{4}$", ErrorMessage = "A placa deve estar no formato ABC1234")]
		public string LicensePlate { get; set; } = null!;

		/// <summary>
		/// Tipo do veículo
		/// </summary>
		[Required(ErrorMessage = "O tipo do veículo é obrigatório")]
		[EnumDataType(typeof(VehicleType), ErrorMessage = "Tipo de veículo inválido")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public VehicleType VehicleType { get; set; }

		/// <summary>
		/// Data e hora da utilização
		/// </summary>
		[Required(ErrorMessage = "A data e hora da utilização são obrigatórias")]
		public DateTime DateTime { get; set; }

		/// <summary>
		/// Praça de pedágio
		/// </summary>
		[Required(ErrorMessage = "A praça de pedágio é obrigatória")]
		[StringLength(100, ErrorMessage = "A praça deve ter no máximo 100 caracteres")]
		public string Plaza { get; set; } = null!;

		/// <summary>
		/// Cidade da praça
		/// </summary>
		[Required(ErrorMessage = "A cidade é obrigatória")]
		[StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
		public string City { get; set; } = null!;

		/// <summary>
		/// Estado da praça
		/// </summary>
		[Required(ErrorMessage = "O estado é obrigatório")]
		[ValidEstadoUF]
		public string State { get; set; } = null!;

		/// <summary>
		/// Valor do pedágio
		/// </summary>
		[Required(ErrorMessage = "O valor do pedágio é obrigatório")]
		[Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
		public decimal Amount { get; set; }
	}
}
