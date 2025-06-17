using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Repository.Interface
{
	public interface ITollUsageRepository
	{
		Task<TollUsage?> GetByIdAsync(Guid id);
		Task<List<TollUsage>> GetByDateAsync(DateTime date);
		Task<List<TollUsage>> GetByMonthAsync(int year, int month);
		Task AddAsync(TollUsage tollUsage);
		Task SaveChangesAsync();

	}
}
