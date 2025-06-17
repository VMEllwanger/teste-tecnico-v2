using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Thunders.TechTest.ApiService.Data;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Repository.Interface;

namespace Thunders.TechTest.ApiService.Repository
{
	[ExcludeFromCodeCoverage]
	public class HourlyCityReportRepository : IHourlyCityReportRepository
	{
		private readonly ApplicationDbContext _context;

		public HourlyCityReportRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<HourlyCityReport?> GetByCityAndDateAndHourAsync(string city, DateTime date, int hour)
		{
			return await _context.HourlyCityReports
				.FirstOrDefaultAsync(r => r.City == city && r.Date.Date == date.Date && r.Hour == hour);
		}

		public async Task<List<HourlyCityReport>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _context.HourlyCityReports
				.Where(r => r.Date >= startDate && r.Date < endDate)
				.OrderBy(r => r.City)
				.ThenBy(r => r.Hour)
				.ToListAsync();
		}

		public async Task AddAsync(HourlyCityReport report)
		{
			await _context.HourlyCityReports.AddAsync(report);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<HourlyCityReport>> GetByReportIdAsync(Guid reportId)
		{
			return await _context.HourlyCityReports
				.Where(x => x.ReportId == reportId)
				.ToListAsync();
		}

		public void Remove(HourlyCityReport report)
		{
			_context.HourlyCityReports.Remove(report);
		}
	}
}
