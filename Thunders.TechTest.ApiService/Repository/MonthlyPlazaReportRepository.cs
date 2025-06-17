using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Thunders.TechTest.ApiService.Data;
using Thunders.TechTest.ApiService.Models.Reports;
using Thunders.TechTest.ApiService.Repository.Interface;

namespace Thunders.TechTest.ApiService.Repository
{
	[ExcludeFromCodeCoverage]
	public class MonthlyPlazaReportRepository : IMonthlyPlazaReportRepository
	{
		private readonly ApplicationDbContext _context;

		public MonthlyPlazaReportRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<MonthlyPlazaReport?> GetByPlazaAndYearAndMonthAsync(string plaza, int year, int month)
		{
			return await _context.MonthlyPlazaReports
				.FirstOrDefaultAsync(r => r.Plaza == plaza && r.Year == year && r.Month == month);
		}

		public async Task<List<MonthlyPlazaReport>> GetByYearAndMonthAsync(int year, int month, int? limit = null)
		{
			var query = _context.MonthlyPlazaReports
				.Where(r => r.Year == year && r.Month == month)
				.OrderByDescending(r => r.TotalAmount);

			if (limit.HasValue)
			{
				query = (IOrderedQueryable<MonthlyPlazaReport>)query.Take(limit.Value);
			}

			return await query.ToListAsync();
		}

		public async Task AddAsync(MonthlyPlazaReport report)
		{
			await _context.MonthlyPlazaReports.AddAsync(report);
		}

		public void Remove(MonthlyPlazaReport report)
		{
			_context.MonthlyPlazaReports.Remove(report);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<MonthlyPlazaReport>> GetByReportIdAsync(Guid reportId)
		{
			return await _context.MonthlyPlazaReports
				.Where(x => x.ReportId == reportId)
				.ToListAsync();
		}
	}
}
