using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Models.Reports;

namespace Thunders.TechTest.ApiService.Mappers;

/// <summary>
/// Mapper para converter entidades de relatório em DTOs
/// </summary>
public static class ReportMapper
{
	/// <summary>
	/// Converte HourlyCityReport para DTO
	/// </summary>
	public static HourlyCityReportDto ToDto(this HourlyCityReport entity)
	{
		return new HourlyCityReportDto
		{
			State = entity.State,
			City = entity.City,
			Date = entity.Date,
			Hour = entity.Hour,
			TotalAmount = entity.TotalAmount,
			TotalVehicles = entity.TotalVehicles
		};
	}

	/// <summary>
	/// Converte lista de HourlyCityReport para DTOs
	/// </summary>
	public static IEnumerable<HourlyCityReportDto> ToDto(this IEnumerable<HourlyCityReport> entities)
	{
		return entities.Select(ToDto);
	}

	/// <summary>
	/// Converte MonthlyPlazaReport para DTO
	/// </summary>
	public static MonthlyPlazaReportDto ToDto(this MonthlyPlazaReport entity)
	{
		return new MonthlyPlazaReportDto
		{
			Plaza = entity.Plaza,
			City = entity.City,
			State = entity.State,
			Year = entity.Year,
			Month = entity.Month,
			TotalAmount = entity.TotalAmount,
			Rank = entity.Rank
		};
	}

	/// <summary>
	/// Converte lista de MonthlyPlazaReport para DTOs
	/// </summary>
	public static IEnumerable<MonthlyPlazaReportDto> ToDto(this IEnumerable<MonthlyPlazaReport> entities)
	{
		return entities.Select(ToDto);
	}

	/// <summary>
	/// Converte VehicleTypeReport para DTO
	/// </summary>
	public static VehicleTypeReportDto ToDto(this VehicleTypeReport entity)
	{
		return new VehicleTypeReportDto
		{
			Plaza = entity.Plaza,
			City = entity.City,
			State = entity.State,
			Date = entity.Date,
			VehicleType = entity.VehicleType,
			VehicleCount = entity.VehicleCount
		};
	}

	/// <summary>
	/// Converte lista de VehicleTypeReport para DTOs
	/// </summary>
	public static IEnumerable<VehicleTypeReportDto> ToDto(this IEnumerable<VehicleTypeReport> entities)
	{
		return entities.Select(ToDto);
	}

	/// <summary>
	/// Converte TollUsage para DTO
	/// </summary>
	public static TollUsageDto ToDto(this TollUsage entity)
	{
		return new TollUsageDto
		{
			LicensePlate = entity.LicensePlate,
			VehicleType = entity.VehicleType,
			DateTime = entity.DateTime,
			Plaza = entity.Plaza,
			City = entity.City,
			State = entity.State,
			Amount = entity.Amount
		};
	}

	/// <summary>
	/// Converte lista de TollUsage para DTOs
	/// </summary>
	public static IEnumerable<TollUsageDto> ToDto(this IEnumerable<TollUsage> entities)
	{
		return entities.Select(ToDto);
	}
}
