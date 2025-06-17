using Microsoft.AspNetCore.Mvc;
using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Mappers;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Services.Interfaces;

namespace Thunders.TechTest.ApiService.Controllers;

/// <summary>
/// Controlador para gerenciamento de relatórios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
	private readonly IReportService _reportService;
	private readonly ILogger<ReportsController> _logger;

	public ReportsController(
		IReportService reportService,
		ILogger<ReportsController> logger)
	{
		_reportService = reportService;
		_logger = logger;
	}

	/// <summary>
	/// Obtém um relatório por hora e cidade pelo ID
	/// </summary>
	/// <param name="reportId">ID do relatório</param>
	/// <returns>Lista de registros por hora e cidade</returns>
	/// <response code="200">Relatório encontrado com sucesso</response>
	/// <response code="404">Relatório não encontrado</response>
	/// <response code="500">Erro interno do servidor</response>
	[HttpGet("hourly-city/{reportId}")]
	[ProducesResponseType(typeof(IEnumerable<HourlyCityReportDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<IEnumerable<HourlyCityReportDto>>> GetHourlyCityReportAsync(Guid reportId)
	{
		try
		{
			var report = await _reportService.GetHourlyCityReportByIdAsync(reportId);
			if (report == null || !report.Any())
			{
				return NotFound($"Relatório {reportId} não encontrado");
			}
			return Ok(report.ToDto());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao buscar relatório por hora e cidade {ReportId}", reportId);
			throw;
		}
	}

	/// <summary>
	/// Obtém um relatório mensal por praça pelo ID
	/// </summary>
	/// <param name="reportId">ID do relatório</param>
	/// <param name="limit">Limite de registros (opcional)</param>
	/// <returns>Lista de registros por praça</returns>
	/// <response code="200">Relatório encontrado com sucesso</response>
	/// <response code="404">Relatório não encontrado</response>
	/// <response code="500">Erro interno do servidor</response>
	[HttpGet("monthly-plaza/{reportId}")]
	[ProducesResponseType(typeof(IEnumerable<MonthlyPlazaReportDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<IEnumerable<MonthlyPlazaReportDto>>> GetMonthlyPlazaReportAsync(
		Guid reportId,
		[FromQuery] int? limit = null)
	{
		try
		{
			if (limit.HasValue && limit.Value <= 0)
			{
				return BadRequest("O limite deve ser maior que zero");
			}

			var report = await _reportService.GetMonthlyPlazaReportByIdAsync(reportId, limit);
			if (report == null || !report.Any())
			{
				return NotFound($"Relatório {reportId} não encontrado");
			}
			return Ok(report.ToDto().OrderBy(x => x.Rank));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao buscar relatório mensal por praça {ReportId}", reportId);
			throw;
		}
	}

	/// <summary>
	/// Obtém um relatório por tipo de veículo pelo ID
	/// </summary>
	/// <param name="reportId">ID do relatório</param>
	/// <returns>Lista de registros por tipo de veículo</returns>
	/// <response code="200">Relatório encontrado com sucesso</response>
	/// <response code="404">Relatório não encontrado</response>
	/// <response code="500">Erro interno do servidor</response>
	[HttpGet("vehicle-type/{reportId}")]
	[ProducesResponseType(typeof(IEnumerable<VehicleTypeReportDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<IEnumerable<VehicleTypeReportDto>>> GetVehicleTypeReportAsync(Guid reportId)
	{
		try
		{
			var report = await _reportService.GetVehicleTypeReportByIdAsync(reportId);
			if (report == null || !report.Any())
			{
				return NotFound($"Relatório {reportId} não encontrado");
			}
			return Ok(report.ToDto());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao buscar relatório por tipo de veículo {ReportId}", reportId);
			throw;
		}
	}

	/// <summary>
	/// Solicita a geração de um relatório
	/// </summary>
	/// <param name="date">Data para gerar o relatório</param>
	/// <param name="reportType">Tipo do relatório</param>
	/// <returns>ID do relatório gerado</returns>
	/// <response code="200">Solicitação processada com sucesso</response>
	/// <response code="400">Parâmetros inválidos</response>
	/// <response code="500">Erro interno do servidor</response>
	[HttpPost("generate")]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<Guid>> GenerateReportAsync(
		[FromQuery] DateTime date,
		[FromQuery] ReportType reportType)
	{
		try
		{
			if (date > DateTime.Now)
			{
				return BadRequest("A data não pode ser futura");
			}

			if (!Enum.IsDefined(typeof(ReportType), reportType))
			{
				return BadRequest("Tipo de relatório inválido");
			}

			var reportId = await _reportService.GenerateReportAsync(date, reportType);
			return Ok(reportId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao solicitar geração de relatório");
			throw;
		}
	}
}
