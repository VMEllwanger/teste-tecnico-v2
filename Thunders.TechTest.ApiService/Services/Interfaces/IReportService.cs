using Thunders.TechTest.ApiService.Models.Reports;

namespace Thunders.TechTest.ApiService.Services.Interfaces;

public interface IReportService
{
	/// <summary>
	/// Gera um novo relatório e retorna seu ID
	/// </summary>
	Task<Guid> GenerateReportAsync(DateTime date, ReportType reportType);

	/// <summary>
	/// Obtém um relatório por hora e cidade pelo ID
	/// </summary>
	Task<IEnumerable<HourlyCityReport>> GetHourlyCityReportByIdAsync(Guid reportId);

	/// <summary>
	/// Obtém um relatório mensal por praça pelo ID
	/// </summary>
	Task<IEnumerable<MonthlyPlazaReport>> GetMonthlyPlazaReportByIdAsync(Guid reportId, int? limit = null);

	/// <summary>
	/// Obtém um relatório por tipo de veículo pelo ID
	/// </summary>
	Task<IEnumerable<VehicleTypeReport>> GetVehicleTypeReportByIdAsync(Guid reportId);
}
