using System.Diagnostics;
using Thunders.TechTest.ApiService.Logging;
using Thunders.TechTest.ApiService.Messaging;
using Thunders.TechTest.ApiService.Metrics;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Repository.Interface;
using Thunders.TechTest.ApiService.Services.Interfaces;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.ApiService.Services
{
	public class ReportService : IReportService
	{
		private readonly IHourlyCityReportRepository _hourlyCityReportRepository;
		private readonly IMonthlyPlazaReportRepository _monthlyPlazaReportRepository;
		private readonly IVehicleTypeReportRepository _vehicleTypeReportRepository;
		private readonly IMessageSender _messageSender;
		private readonly ILogger<ReportService> _logger;
		private readonly ActivitySource _activitySource;

		public ReportService(
			IHourlyCityReportRepository hourlyCityReportRepository,
			IMonthlyPlazaReportRepository monthlyPlazaReportRepository,
			IVehicleTypeReportRepository vehicleTypeReportRepository,
			IMessageSender messageSender,
			ILogger<ReportService> logger,
			ActivitySource activitySource)
		{
			_hourlyCityReportRepository = hourlyCityReportRepository;
			_monthlyPlazaReportRepository = monthlyPlazaReportRepository;
			_vehicleTypeReportRepository = vehicleTypeReportRepository;
			_messageSender = messageSender;
			_logger = logger;
			_activitySource = activitySource;
		}

		public async Task<IEnumerable<HourlyCityReport>> GetHourlyCityReportAsync(DateTime date)
		{
			using var activity = _activitySource.StartActivity("GetHourlyCityReport");
			activity?.SetTag("date", date.ToString("yyyy-MM-dd"));

			var additionalData = new Dictionary<string, object>
			{
				["Date"] = date
			};

			_logger.LogOperationStart("GetHourlyCityReport", "Buscando relatório por hora e cidade", additionalData);

			try
			{
				var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Local).ToUniversalTime();
				var utcDateEnd = utcDate.AddDays(1);

				var reports = await _hourlyCityReportRepository.GetByDateRangeAsync(utcDate, utcDateEnd);

				additionalData["ReportCount"] = reports.Count;
				_logger.LogOperationEnd("GetHourlyCityReport", "Relatório por hora e cidade obtido com sucesso", additionalData);

				return reports;
			}
			catch (Exception ex)
			{
				BusinessMetrics.ProcessingErrors.Add(1);
				_logger.LogOperationError("GetHourlyCityReport", "Erro ao buscar relatório por hora e cidade", ex, additionalData);
				activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
				throw;
			}
		}

		public async Task<IEnumerable<MonthlyPlazaReport>> GetMonthlyPlazaReportAsync(int year, int month, int? limit = null)
		{
			using var activity = _activitySource.StartActivity("GetMonthlyPlazaReport");
			activity?.SetTag("year", year.ToString());
			activity?.SetTag("month", month.ToString());
			activity?.SetTag("limit", limit?.ToString() ?? "null");

			var additionalData = new Dictionary<string, object>
			{
				["Year"] = year,
				["Month"] = month,
				["Limit"] = limit
			};

			_logger.LogOperationStart("GetMonthlyPlazaReport", "Buscando relatório mensal de praças", additionalData);

			try
			{
				var reports = await _monthlyPlazaReportRepository.GetByYearAndMonthAsync(year, month, limit);

				additionalData["ReportCount"] = reports.Count;
				_logger.LogOperationEnd("GetMonthlyPlazaReport", "Relatório mensal de praças obtido com sucesso", additionalData);

				return reports;
			}
			catch (Exception ex)
			{
				BusinessMetrics.ProcessingErrors.Add(1);
				_logger.LogOperationError("GetMonthlyPlazaReport", "Erro ao buscar relatório mensal de praças", ex, additionalData);
				activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
				throw;
			}
		}

		public async Task<IEnumerable<VehicleTypeReport>> GetVehicleTypeReportAsync(DateTime date, string? plaza = null)
		{
			using var activity = _activitySource.StartActivity("GetVehicleTypeReport");
			activity?.SetTag("date", date.ToString("yyyy-MM-dd"));
			activity?.SetTag("plaza", plaza ?? "null");

			var additionalData = new Dictionary<string, object>
			{
				["Date"] = date,
				["Plaza"] = plaza
			};

			_logger.LogOperationStart("GetVehicleTypeReport", "Buscando relatório de tipos de veículos", additionalData);

			try
			{
				var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Local).ToUniversalTime();
				var utcDateEnd = utcDate.AddDays(1);

				var reports = await _vehicleTypeReportRepository.GetByDateRangeAsync(utcDate, utcDateEnd, plaza);

				additionalData["ReportCount"] = reports.Count;
				_logger.LogOperationEnd("GetVehicleTypeReport", "Relatório de tipos de veículos obtido com sucesso", additionalData);

				return reports;
			}
			catch (Exception ex)
			{
				BusinessMetrics.ProcessingErrors.Add(1);
				_logger.LogOperationError("GetVehicleTypeReport", "Erro ao buscar relatório de tipos de veículos", ex, additionalData);
				activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
				throw;
			}
		}

		public async Task<Guid> GenerateReportAsync(DateTime date, ReportType reportType)
		{
			_logger.LogInformation("Iniciando geração de relatório do tipo {ReportType} para a data {Date}", reportType, date);

			var reportId = Guid.NewGuid();

			_logger.LogInformation("Enviando mensagem para geração de relatório: {ReportId}, {Data}, {TipoRelatorio}",
				reportId, date, reportType);

			await _messageSender.SendLocal(new GenerateReportMessage(reportId, date, reportType));

			_logger.LogInformation("Mensagem enviada com sucesso para geração do relatório {ReportId}", reportId);
			return reportId;
		}

		public async Task<IEnumerable<HourlyCityReport>> GetHourlyCityReportByIdAsync(Guid reportId)
		{
			_logger.LogInformation("Buscando relatório por hora e cidade com ID {ReportId}", reportId);
			return await _hourlyCityReportRepository.GetByReportIdAsync(reportId);
		}

		public async Task<IEnumerable<MonthlyPlazaReport>> GetMonthlyPlazaReportByIdAsync(Guid reportId, int? limit = null)
		{
			_logger.LogInformation("Buscando relatório mensal por praça com ID {ReportId}", reportId);
			var reports = await _monthlyPlazaReportRepository.GetByReportIdAsync(reportId);

			if (limit.HasValue)
			{
				return reports.Take(limit.Value);
			}

			return reports;
		}

		public async Task<IEnumerable<VehicleTypeReport>> GetVehicleTypeReportByIdAsync(Guid reportId)
		{
			_logger.LogInformation("Buscando relatório por tipo de veículo com ID {ReportId}", reportId);
			return await _vehicleTypeReportRepository.GetByReportIdAsync(reportId);
		}
	}
}
