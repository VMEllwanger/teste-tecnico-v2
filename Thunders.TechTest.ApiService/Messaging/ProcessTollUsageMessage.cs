using System.Diagnostics.CodeAnalysis;

namespace Thunders.TechTest.ApiService.Messaging;
[ExcludeFromCodeCoverage]
public class ProcessTollUsageMessage
{
	public Guid TollUsageId { get; }

	public ProcessTollUsageMessage(Guid tollUsageId)
	{
		TollUsageId = tollUsageId;
	}
}