using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Repository.Interface;
using Thunders.TechTest.ApiService.Services.Interfaces;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.ApiService.Services
{
	public class TollUsageService : ITollUsageService
	{
		private readonly ITollUsageRepository _tollUsageRepository;
		private readonly ILogger<TollUsageService> _logger;

		public TollUsageService(
			ITollUsageRepository tollUsageRepository,
			IHourlyCityReportRepository hourlyCityReportRepository,
			IMonthlyPlazaReportRepository monthlyPlazaReportRepository,
			IVehicleTypeReportRepository vehicleTypeReportRepository,
			IMessageSender messageSender,
			ILogger<TollUsageService> logger)
		{
			_tollUsageRepository = tollUsageRepository;
			_logger = logger;
		}

		public async Task<Guid> CreateTollUsageAsync(TollUsageRequestDto request)
		{
			try
			{
				_logger.LogInformation("Criando novo registro de pedágio para a placa {Placa}", request.LicensePlate);

				var tollUsage = new TollUsage
				{
					Id = Guid.NewGuid(),
					LicensePlate = request.LicensePlate,
					Amount = request.Amount,
					DateTime = request.DateTime.Kind == DateTimeKind.Unspecified
						? DateTime.SpecifyKind(request.DateTime, DateTimeKind.Utc)
						: request.DateTime.ToUniversalTime(),
					Plaza = request.Plaza,
					City = request.City,
					State = request.State.ToUpper(),
					VehicleType = request.VehicleType,
					CreatedAt = DateTime.UtcNow
				};

				await _tollUsageRepository.AddAsync(tollUsage);
				await _tollUsageRepository.SaveChangesAsync();

				_logger.LogInformation("Registro de pedágio criado com sucesso com o identificador {IdRegistro}", tollUsage.Id);
				return tollUsage.Id;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao criar registro de pedágio para a placa {Placa}", request.LicensePlate);
				throw;
			}
		}

		public async Task<TollUsage?> GetByIdAsync(Guid id)
		{
			try
			{
				_logger.LogInformation("Buscando registro de pedágio pelo identificador {IdRegistro}", id);
				return await _tollUsageRepository.GetByIdAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao buscar registro de pedágio pelo identificador {IdRegistro}", id);
				throw;
			}
		}

		public async Task<IEnumerable<TollUsage>> GetTollUsagesAsync(DateTime date)
		{
			try
			{
				_logger.LogInformation("Buscando registros de pedágio para a data {Data}", date);
				return await _tollUsageRepository.GetByDateAsync(date);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao buscar registros de pedágio para a data {Data}", date);
				throw;
			}
		}

	}
}
