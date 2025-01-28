using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Thunders.TechTest.ApiService.Metrics;
[ExcludeFromCodeCoverage]
public static class BusinessMetrics
{
	public static readonly Meter Meter = new("Thunders.TechTest.ApiService.Business", "1.0.0");

	// Métricas de pedágio
	public static readonly Counter<double> TotalRevenue = Meter.CreateCounter<double>(
		"toll_revenue_total",
		"BRL",
		"Total revenue from toll usage");

	public static readonly Counter<int> TotalVehicles = Meter.CreateCounter<int>(
		"toll_vehicles_total",
		"Number",
		"Total number of vehicles processed");

	// Métricas por tipo de veículo
	public static readonly Counter<double> RevenueByVehicleType = Meter.CreateCounter<double>(
		"toll_revenue_by_vehicle_type",
		"BRL",
		"Revenue by vehicle type");

	public static readonly Counter<int> VehiclesByType = Meter.CreateCounter<int>(
		"toll_vehicles_by_type",
		"Number",
		"Number of vehicles by type");

	// Métricas por praça
	public static readonly Counter<double> RevenueByPlaza = Meter.CreateCounter<double>(
		"toll_revenue_by_plaza",
		"BRL",
		"Revenue by plaza");

	public static readonly Counter<int> VehiclesByPlaza = Meter.CreateCounter<int>(
		"toll_vehicles_by_plaza",
		"Number",
		"Number of vehicles by plaza");

	// Métricas por cidade
	public static readonly Counter<double> RevenueByCity = Meter.CreateCounter<double>(
		"toll_revenue_by_city",
		"BRL",
		"Revenue by city");

	public static readonly Counter<int> VehiclesByCity = Meter.CreateCounter<int>(
		"toll_vehicles_by_city",
		"Number",
		"Number of vehicles by city");

	// Métricas de tempo
	public static readonly Histogram<double> ProcessingTimeByVehicleType = Meter.CreateHistogram<double>(
		"toll_processing_time_by_vehicle_type_seconds",
		"Seconds",
		"Processing time by vehicle type");

	public static readonly Histogram<double> ProcessingTimeByPlaza = Meter.CreateHistogram<double>(
		"toll_processing_time_by_plaza_seconds",
		"Seconds",
		"Processing time by plaza");

	// Métricas de erro
	public static readonly Counter<int> ProcessingErrors = Meter.CreateCounter<int>(
		"toll_processing_errors_total",
		"Number",
		"Total number of processing errors");

	public static readonly Counter<int> ValidationErrors = Meter.CreateCounter<int>(
		"toll_validation_errors_total",
		"Number",
		"Total number of validation errors");

	// Métricas de relatórios
	public static readonly Counter<int> ReportsGenerated = Meter.CreateCounter<int>(
		"toll_reports_generated_total",
		"Number",
		"Total number of reports generated");

	public static readonly Histogram<double> ReportGenerationTime = Meter.CreateHistogram<double>(
		"toll_report_generation_time_seconds",
		"Seconds",
		"Time taken to generate reports");

	// Métricas de performance
	public static readonly ObservableGauge<int> ActiveProcessingJobs = Meter.CreateObservableGauge(
		"toll_active_processing_jobs",
		() => GetActiveProcessingJobsCount(),
		"Number",
		"Number of active processing jobs");

	public static readonly ObservableGauge<int> PendingReports = Meter.CreateObservableGauge(
		"toll_pending_reports",
		() => GetPendingReportsCount(),
		"Number",
		"Number of pending reports");

	private static int GetActiveProcessingJobsCount()
	{
		// Implementar lógica para obter o número de jobs ativos
		return 0;
	}

	private static int GetPendingReportsCount()
	{
		// Implementar lógica para obter o número de relatórios pendentes
		return 0;
	}
}
