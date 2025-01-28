using Microsoft.AspNetCore.Mvc;
using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Mappers;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Services.Interfaces;

namespace Thunders.TechTest.ApiService.Controllers;

/// <summary>
/// Controlador para gerenciamento de registros de pedágio
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TollUsageController : ControllerBase
{
	private readonly ITollUsageService _tollUsageService;
	private readonly ILogger<TollUsageController> _logger;

	public TollUsageController(
		ITollUsageService tollUsageService,
		ILogger<TollUsageController> logger)
	{
		_tollUsageService = tollUsageService;
		_logger = logger;
	}

	/// <summary>
	/// Cria um novo registro de pedágio
	/// </summary>
	/// <param name="tollUsage">Dados do registro de pedágio</param>
	/// <returns>ID do registro criado</returns>
	/// <response code="200">Registro criado com sucesso</response>
	/// <response code="400">Dados inválidos</response>
	/// <response code="500">Erro interno do servidor</response>
	[HttpPost]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<Guid>> CreateAsync([FromBody] TollUsageRequestDto tollUsage)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var id = await _tollUsageService.CreateTollUsageAsync(tollUsage);
			return Ok(id);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao criar registro de pedágio");
			throw;
		}
	}

	/// <summary>
	/// Busca registros de pedágio por data
	/// </summary>
	/// <param name="date">Data para filtrar os registros</param>
	/// <returns>Lista de registros de pedágio</returns>
	/// <response code="200">Registros encontrados</response>
	/// <response code="400">Data inválida</response>
	/// <response code="500">Erro interno do servidor</response>
	[HttpGet("{date}")]
	[ProducesResponseType(typeof(IEnumerable<TollUsageDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<IEnumerable<TollUsageDto>>> GetByDateAsync(DateTime date)
	{
		try
		{
			if (date.Date > DateTime.Now)
			{
				return BadRequest("A data não pode ser futura");
			}

			var localDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Local);
			var tollUsages = await _tollUsageService.GetTollUsagesAsync(localDate);
			return Ok(tollUsages.ToDto());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao buscar registros de pedágio");
			throw;
		}
	}
}
