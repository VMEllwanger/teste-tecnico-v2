using Thunders.TechTest.ApiService.Models.Reports;

namespace Thunders.TechTest.ApiService.Repository.Interface
{
	public interface IHourlyCityReportRepository
	{
		Task<HourlyCityReport?> GetByCityAndDateAndHourAsync(string city, DateTime date, int hour);
		Task<List<HourlyCityReport>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
		Task AddAsync(HourlyCityReport report);
		Task SaveChangesAsync();
		Task<IEnumerable<HourlyCityReport>> GetByReportIdAsync(Guid reportId);

		void Remove(HourlyCityReport report);
	}
}
