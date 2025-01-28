using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Thunders.TechTest.ApiService.Metrics;
[ExcludeFromCodeCoverage]
public static class TollMetrics
{
	private static readonly Meter Meter = new("Thunders.TechTest.ApiService", "1.0.0");

	// Contadores
	public static readonly Counter<int> TollUsageProcessed = Meter.CreateCounter<int>(
		"toll_usage_processed_total",
		"Number",
		"Total number of toll usage records processed");

	public static readonly Counter<int> ReportsGenerated = Meter.CreateCounter<int>(
		"reports_generated_total",
		"Number",
		"Total number of reports generated");

	// Histogramas
	public static readonly Histogram<double> TollUsageProcessingTime = Meter.CreateHistogram<double>(
		"toll_usage_processing_time_seconds",
		"Seconds",
		"Time taken to process toll usage records");

	public static readonly Histogram<double> ReportGenerationTime = Meter.CreateHistogram<double>(
		"report_generation_time_seconds",
		"Seconds",
		"Time taken to generate reports");

	// Observações
	public static readonly ObservableGauge<int> ActiveTollUsages = Meter.CreateObservableGauge(
		"active_toll_usages",
		() => GetActiveTollUsagesCount(),
		"Number",
		"Number of active toll usage records");

	private static int GetActiveTollUsagesCount()
	{
		// Implementar lógica para obter o número de registros de pedágio ativos
		return 0;
	}
}
