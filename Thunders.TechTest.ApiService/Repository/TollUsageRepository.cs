using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Thunders.TechTest.ApiService.Data;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Repository.Interface;

namespace Thunders.TechTest.ApiService.Repository
{
	[ExcludeFromCodeCoverage]
	public class TollUsageRepository : ITollUsageRepository
	{
		private readonly ApplicationDbContext _context;

		public TollUsageRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<TollUsage?> GetByIdAsync(Guid id)
		{
			return await _context.TollUsages.FindAsync(id);
		}

		public async Task<List<TollUsage>> GetByDateAsync(DateTime date)
		{
			var utcDate = date.ToUniversalTime();
			return await _context.TollUsages
				.Where(t => t.DateTime.Date == utcDate.Date)
				.ToListAsync();
		}

		public async Task<List<TollUsage>> GetByMonthAsync(int year, int month)
		{
			return await _context.TollUsages
				.Where(t => t.DateTime.Year == year && t.DateTime.Month == month)
				.ToListAsync();
		}

		public async Task AddAsync(TollUsage tollUsage)
		{
			await _context.TollUsages.AddAsync(tollUsage);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

	}
}
