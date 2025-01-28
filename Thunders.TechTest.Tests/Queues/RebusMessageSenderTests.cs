using Microsoft.Extensions.Logging;
using NSubstitute;
using Rebus.Bus;
using Thunders.TechTest.ApiService.Services;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.Tests.Queues
{
	public class RebusMessageSenderTests
	{
		private readonly IBus _bus;
		private readonly ILogger<RebusMessageSender> _logger;
		private readonly RebusMessageSender _sender;

		public RebusMessageSenderTests()
		{
			_bus = Substitute.For<IBus>();
			_logger = Substitute.For<ILogger<RebusMessageSender>>();
			_sender = new RebusMessageSender(_bus, _logger);
		}

		[Fact]
		public async Task SendLocal_ShouldCallBusSendLocal()
		{
			// Arrange
			var message = new { Id = 1, Name = "Test" };
			_bus.SendLocal(Arg.Any<object>()).Returns(Task.CompletedTask);

			// Act
			await _sender.SendLocal(message);

			// Assert
			await _bus.Received(1).SendLocal(message);
		}

		[Fact]
		public async Task SendLocal_WhenException_ShouldPropagateException()
		{
			// Arrange
			var message = new { Id = 1, Name = "Test" };
			var expectedException = new Exception("Erro ao enviar mensagem");
			_bus.SendLocal(Arg.Any<object>()).Returns(Task.FromException(expectedException));

			// Act & Assert
			var exception = await Assert.ThrowsAsync<Exception>(() => _sender.SendLocal(message));
			Assert.Equal(expectedException.Message, exception.Message);
		}

		[Fact]
		public async Task Publish_ShouldCallBusPublish()
		{
			// Arrange
			var message = new { Id = 1, Name = "Test" };
			_bus.Publish(Arg.Any<object>()).Returns(Task.CompletedTask);

			// Act
			await _sender.Publish(message);

			// Assert
			await _bus.Received(1).Publish(message);
		}

		[Fact]
		public async Task Publish_WhenException_ShouldPropagateException()
		{
			// Arrange
			var message = new { Id = 1, Name = "Test" };
			var expectedException = new Exception("Erro ao publicar mensagem");
			_bus.Publish(Arg.Any<object>()).Returns(Task.FromException(expectedException));

			// Act & Assert
			var exception = await Assert.ThrowsAsync<Exception>(() => _sender.Publish(message));
			Assert.Equal(expectedException.Message, exception.Message);
		}

		[Fact]
		public async Task SendLocal_WithNullMessage_ShouldThrowArgumentNullException()
		{
			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => _sender.SendLocal(null));
		}

		[Fact]
		public async Task Publish_WithNullMessage_ShouldThrowArgumentNullException()
		{
			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => _sender.Publish(null));
		}

		[Fact]
		public async Task SendLocal_WithDifferentMessageTypes_ShouldCallBusSendLocal()
		{
			// Arrange
			var message1 = new { Id = 1, Name = "Test1" };
			var message2 = new { Id = 2, Name = "Test2" };
			_bus.SendLocal(Arg.Any<object>()).Returns(Task.CompletedTask);

			// Act
			await _sender.SendLocal(message1);
			await _sender.SendLocal(message2);

			// Assert
			await _bus.Received(1).SendLocal(message1);
			await _bus.Received(1).SendLocal(message2);
		}

		[Fact]
		public async Task Publish_WithDifferentMessageTypes_ShouldCallBusPublish()
		{
			// Arrange
			var message1 = new { Id = 1, Name = "Test1" };
			var message2 = new { Id = 2, Name = "Test2" };
			_bus.Publish(Arg.Any<object>()).Returns(Task.CompletedTask);

			// Act
			await _sender.Publish(message1);
			await _sender.Publish(message2);

			// Assert
			await _bus.Received(1).Publish(message1);
			await _bus.Received(1).Publish(message2);
		}
	}
}
