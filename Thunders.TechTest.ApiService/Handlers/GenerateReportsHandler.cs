using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using System.Diagnostics;
using Thunders.TechTest.ApiService.Logging;
using Thunders.TechTest.ApiService.Messaging;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Repository.Interface;

namespace Thunders.TechTest.ApiService.Handlers;

public class GenerateReportsHandler : IHandleMessages<GenerateReportMessage>
{
	private readonly ITollUsageRepository _tollUsageRepository;
	private readonly IHourlyCityReportRepository _hourlyCityReportRepository;
	private readonly IMonthlyPlazaReportRepository _monthlyPlazaReportRepository;
	private readonly IVehicleTypeReportRepository _vehicleTypeReportRepository;
	private readonly ILogger<GenerateReportsHandler> _logger;
	private readonly ActivitySource _activitySource;

	public GenerateReportsHandler(
		ITollUsageRepository tollUsageRepository,
		IHourlyCityReportRepository hourlyCityReportRepository,
		IMonthlyPlazaReportRepository monthlyPlazaReportRepository,
		IVehicleTypeReportRepository vehicleTypeReportRepository,
		ILogger<GenerateReportsHandler> logger,
		ActivitySource activitySource)
	{
		_tollUsageRepository = tollUsageRepository;
		_hourlyCityReportRepository = hourlyCityReportRepository;
		_monthlyPlazaReportRepository = monthlyPlazaReportRepository;
		_vehicleTypeReportRepository = vehicleTypeReportRepository;
		_logger = logger;
		_activitySource = activitySource;
	}

	public async Task Handle(GenerateReportMessage message)
	{
		using var activity = _activitySource.StartActivity("GenerateReport");
		activity?.SetTag("reportId", message.ReportId.ToString());
		activity?.SetTag("date", message.Date.ToString("yyyy-MM-dd"));
		activity?.SetTag("reportType", message.ReportType.ToString());

		var additionalData = new Dictionary<string, object>
		{
			["ReportId"] = message.ReportId,
			["Date"] = message.Date,
			["ReportType"] = message.ReportType
		};

		_logger.LogInformation("Iniciando processamento do relatório {ReportId}: {Data}, {TipoRelatorio}",
			message.ReportId, message.Date, message.ReportType);
		_logger.LogOperationStart("GenerateReport", "Gerando relatório", additionalData);

		try
		{
			switch (message.ReportType)
			{
				case ReportType.HourlyCity:
					_logger.LogInformation("Gerando relatório por hora e cidade");
					await GenerateHourlyCityReport(message.ReportId, message.Date);
					break;
				case ReportType.MonthlyPlaza:
					_logger.LogInformation("Gerando relatório mensal por praça");
					await GenerateMonthlyPlazaReport(message.ReportId, message.Date);
					break;
				case ReportType.VehicleType:
					_logger.LogInformation("Gerando relatório por tipo de veículo");
					await GenerateVehicleTypeReport(message.ReportId, message.Date);
					break;
				default:
					throw new ArgumentException($"Tipo de relatório não suportado: {message.ReportType}");
			}

			_logger.LogInformation("Relatório {ReportId} gerado com sucesso", message.ReportId);
			_logger.LogOperationEnd("GenerateReport", "Relatório gerado com sucesso", additionalData);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao gerar relatório {ReportId} do tipo {TipoRelatorio} para a data {Data}",
				message.ReportId, message.ReportType, message.Date);
			activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			throw;
		}
	}

	private async Task GenerateHourlyCityReport(Guid reportId, DateTime date)
	{
		_logger.LogInformation("Gerando relatório por hora e cidade para a data: {Date}", date);

		var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Local).ToUniversalTime();
		var utcDateEnd = utcDate.AddDays(1);

		_logger.LogInformation("Data UTC: {UtcDate}, Data UTC fim: {UtcDateEnd}", utcDate, utcDateEnd);

		var usages = await _tollUsageRepository.GetByDateAsync(date);
		var reports = usages
			.Where(x => x.DateTime.Date >= utcDate.Date && x.DateTime.Date < utcDateEnd)
			.GroupBy(x => new { x.City, x.DateTime.Hour })
			.Select(g => new HourlyCityReport
			{
				ReportId = reportId,
				City = g.Key.City,
				Date = utcDate,
				Hour = g.Key.Hour,
				State = g.First().State,
				TotalAmount = g.Sum(x => x.Amount),
				TotalVehicles = g.Count(),
				CreatedAt = DateTime.UtcNow
			})
			.ToList();

		var existingReports = await _hourlyCityReportRepository.GetByDateRangeAsync(utcDate.Date, utcDateEnd.Date);
		if (existingReports.Any())
		{
			foreach (var report in existingReports)
			{
				_hourlyCityReportRepository.Remove(report);
			}
			await _hourlyCityReportRepository.SaveChangesAsync();
		}

		foreach (var report in reports)
		{
			await _hourlyCityReportRepository.AddAsync(report);
		}
		await _hourlyCityReportRepository.SaveChangesAsync();

		_logger.LogInformation("Relatório por hora e cidade gerado com sucesso");
	}

	private async Task GenerateMonthlyPlazaReport(Guid reportId, DateTime date)
	{
		_logger.LogInformation("Gerando relatório mensal por praça para a data: {Date}", date);

		var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Local).ToUniversalTime();
		var utcDateEnd = utcDate.AddDays(1);

		var existingReports = await _monthlyPlazaReportRepository.GetByYearAndMonthAsync(date.Year, date.Month);
		if (existingReports.Any())
		{
			_logger.LogInformation("Removendo {Count} relatórios mensais existentes para {Year}/{Month}",
				existingReports.Count, date.Year, date.Month);

			foreach (var existingReport in existingReports)
			{
				_monthlyPlazaReportRepository.Remove(existingReport);
			}
			await _monthlyPlazaReportRepository.SaveChangesAsync();
		}

		var startOfMonth = new DateTime(date.Year, date.Month, 1);
		var endOfMonth = startOfMonth.AddMonths(1);

		var usages = await _tollUsageRepository.GetByMonthAsync(date.Year, date.Month);

		var reports = usages
			.GroupBy(x => x.Plaza)
			.Select(g => new MonthlyPlazaReport
			{
				ReportId = reportId,
				Plaza = g.Key,
				City = g.First().City,
				State = g.First().State,
				Year = date.Year,
				Month = date.Month,
				TotalAmount = g.Sum(x => x.Amount),
				Rank = 0,
				CreatedAt = DateTime.UtcNow
			})
			.OrderByDescending(x => x.TotalAmount)
			.ToList();

		for (int i = 0; i < reports.Count; i++)
		{
			reports[i].Rank = i + 1;
		}

		_logger.LogInformation("Adicionando {Count} novos relatórios mensais para {Year}/{Month}",
			reports.Count, date.Year, date.Month);

		foreach (var report in reports)
		{
			await _monthlyPlazaReportRepository.AddAsync(report);
		}
		await _monthlyPlazaReportRepository.SaveChangesAsync();

		_logger.LogInformation("Relatório mensal por praça gerado com sucesso");
	}

	private async Task GenerateVehicleTypeReport(Guid reportId, DateTime date)
	{

		_logger.LogInformation("Gerando relatório por tipo de veículo para a data: {Date}", date);

		var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Local).ToUniversalTime();
		var utcDateEnd = utcDate.AddDays(1);

		_logger.LogInformation("Data UTC: {UtcDate}, Data UTC fim: {UtcDateEnd}", utcDate, utcDateEnd);

		var existingReports = await _vehicleTypeReportRepository.GetByDateRangeAsync(utcDate.Date, utcDateEnd.Date);
		if (existingReports.Any())
		{
			foreach (var report in existingReports)
			{
				_vehicleTypeReportRepository.Remove(report);

			}

			await _vehicleTypeReportRepository.SaveChangesAsync();
		}

		var usages = await _tollUsageRepository.GetByDateAsync(date);
		var grouped = usages
			.Where(x => x.DateTime.Date >= utcDate.Date && x.DateTime.Date < utcDateEnd.Date)
			.GroupBy(x => new { x.Plaza, x.City, x.VehicleType, x.State })
			.Select(g => new VehicleTypeReport
			{
				ReportId = reportId,
				Plaza = g.Key.Plaza,
				City = g.Key.City,
				VehicleType = g.Key.VehicleType,
				Date = utcDate.Date,
				VehicleCount = g.Count(),
				State = g.Key.State,
				CreatedAt = DateTime.UtcNow
			})
			.ToList();

		foreach (var report in grouped)
		{
			await _vehicleTypeReportRepository.AddAsync(report);
		}
		await _vehicleTypeReportRepository.SaveChangesAsync();

		_logger.LogInformation("Relatório por tipo de veículo gerado com sucesso");

	}
}
