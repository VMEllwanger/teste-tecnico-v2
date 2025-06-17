using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Models.Reports;

namespace Thunders.TechTest.ApiService.Repository.Interface
{
	public interface IVehicleTypeReportRepository
	{
		Task<VehicleTypeReport?> GetByVehicleTypeAndDateAsync(VehicleType vehicleType, DateTime date);
		Task<List<VehicleTypeReport>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string? plaza = null);
		Task AddAsync(VehicleTypeReport report);
		Task SaveChangesAsync();
		Task<IEnumerable<VehicleTypeReport>> GetByReportIdAsync(Guid reportId);
		void Remove(VehicleTypeReport report);
	}
}
