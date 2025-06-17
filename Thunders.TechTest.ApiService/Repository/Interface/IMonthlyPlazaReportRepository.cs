using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thunders.TechTest.ApiService.Models.Reports;

namespace Thunders.TechTest.ApiService.Repository.Interface
{
	public interface IMonthlyPlazaReportRepository
	{
		Task<MonthlyPlazaReport?> GetByPlazaAndYearAndMonthAsync(string plaza, int year, int month);
		Task<List<MonthlyPlazaReport>> GetByYearAndMonthAsync(int year, int month, int? limit = null);
		Task AddAsync(MonthlyPlazaReport report);
		void Remove(MonthlyPlazaReport report);
		Task SaveChangesAsync();
		Task<IEnumerable<MonthlyPlazaReport>> GetByReportIdAsync(Guid reportId);
	}
}
