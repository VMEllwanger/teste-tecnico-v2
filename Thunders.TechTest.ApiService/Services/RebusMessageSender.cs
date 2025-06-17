using Rebus.Bus;
using Thunders.TechTest.ApiService.Messaging;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.ApiService.Services;

public class RebusMessageSender : IMessageSender
{
	private readonly IBus _bus;
	private readonly ILogger<RebusMessageSender> _logger;

	public RebusMessageSender(IBus bus, ILogger<RebusMessageSender> logger)
	{
		_bus = bus;
		_logger = logger;
	}

	public async Task SendLocal(object mensagem)
	{
		if (mensagem == null)
		{
			throw new ArgumentNullException(nameof(mensagem), "A mensagem não pode ser nula.");
		}

		_logger.LogInformation("Enviando mensagem local do tipo: {TipoMensagem}", mensagem.GetType().Name);
		try
		{
			await _bus.SendLocal(mensagem);
			_logger.LogInformation("Mensagem local enviada com sucesso do tipo: {TipoMensagem}", mensagem.GetType().Name);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ocorreu um erro ao enviar a mensagem local do tipo: {TipoMensagem}", mensagem.GetType().Name);
			throw;
		}
	}

	public async Task Publish(object mensagem)
	{
		if (mensagem == null)
		{
			throw new ArgumentNullException(nameof(mensagem), "A mensagem não pode ser nula.");
		}

		_logger.LogInformation("Publicando mensagem do tipo: {TipoMensagem}", mensagem.GetType().Name);
		try
		{
			await _bus.Publish(mensagem);
			_logger.LogInformation("Mensagem publicada com sucesso do tipo: {TipoMensagem}", mensagem.GetType().Name);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ocorreu um erro ao publicar a mensagem do tipo: {TipoMensagem}", mensagem.GetType().Name);
			throw;
		}
	}
}

