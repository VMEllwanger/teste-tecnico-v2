using Thunders.TechTest.ApiService.Dtos;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Services.Interfaces
{
	public interface ITollUsageService
	{
		Task<Guid> CreateTollUsageAsync(TollUsageRequestDto tollUsage);
		Task<TollUsage?> GetByIdAsync(Guid id);
		Task<IEnumerable<TollUsage>> GetTollUsagesAsync(DateTime date);
	}
}
