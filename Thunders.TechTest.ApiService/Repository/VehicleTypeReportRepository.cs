using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Thunders.TechTest.ApiService.Data;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Repository.Interface;

namespace Thunders.TechTest.ApiService.Repository
{
	[ExcludeFromCodeCoverage]
	public class VehicleTypeReportRepository : IVehicleTypeReportRepository
	{
		private readonly ApplicationDbContext _context;

		public VehicleTypeReportRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<VehicleTypeReport?> GetByVehicleTypeAndDateAsync(VehicleType vehicleType, DateTime date)
		{
			return await _context.VehicleTypeReports
				.FirstOrDefaultAsync(r => r.VehicleType == vehicleType && r.Date.Date == date.Date);
		}

		public async Task<List<VehicleTypeReport>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string? plaza = null)
		{
			return await _context.VehicleTypeReports
				.Where(r => r.Date.Date >= startDate && r.Date.Date < endDate)
				.ToListAsync();

		}

		public async Task AddAsync(VehicleTypeReport report)
		{
			await _context.VehicleTypeReports.AddAsync(report);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<VehicleTypeReport>> GetByReportIdAsync(Guid reportId)
		{
			return await _context.VehicleTypeReports
				.Where(x => x.ReportId == reportId)
				.ToListAsync();
		}

		public void Remove(VehicleTypeReport report)
		{
			_context.VehicleTypeReports.Remove(report);
		}
	}
}
